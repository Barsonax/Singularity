using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Singularity.Exceptions;
using Singularity.Expressions;
using Singularity.FastExpressionCompiler;

namespace Singularity.Graph.Resolvers
{
    internal class ResolverPipeline : IResolverPipeline
    {
        IReadOnlyDictionary<Type, Dependency> IResolverPipeline.Dependencies => Dependencies;
        private Dictionary<Type, Dependency> Dependencies { get; }
        public object SyncRoot { get; }
        private readonly IDependencyResolver[] _resolvers;
        private readonly IResolverPipeline? _parentPipeline;
        private readonly Scoped _containerScope;
        private readonly ExpressionGenerator _expressionGenerator = new ExpressionGenerator();
        private readonly SingularitySettings _settings;

        public ResolverPipeline(Dictionary<Type, Dependency> dependencies, IDependencyResolver[] resolvers, Scoped containerScope, SingularitySettings settings, IResolverPipeline? parentPipeline)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _resolvers = resolvers;
            _parentPipeline = parentPipeline;
            SyncRoot = parentPipeline?.SyncRoot ?? new object();
            _containerScope = containerScope;
            Dependencies = dependencies;

            if (parentPipeline != null)
            {
                CheckChildBindings(parentPipeline, dependencies, SyncRoot);
            }
        }

        public Dependency? TryGetDependency(Type type)
        {
            lock (SyncRoot)
            {
                if (Dependencies.TryGetValue(type, out Dependency parent)) return parent;

                foreach (IDependencyResolver dependencyResolver in _resolvers)
                {
                    IEnumerable<Dependency>? resolvedDependencies = dependencyResolver.Resolve(this, type);
                    if (resolvedDependencies != null)
                    {
                        foreach (Dependency resolvedDependency in resolvedDependencies)
                        {
                            Dependencies.Add(resolvedDependency.Registration.DependencyType, resolvedDependency);
                        }
                        return TryGetDependency(type);
                    }
                }

                return _parentPipeline?.TryGetDependency(type);
            }
        }

        public Dependency GetDependency(Type type)
        {
            Dependency? dependency = TryGetDependency(type);
            if (dependency == null) throw new DependencyNotFoundException(type);
            return dependency;
        }

        public void ResolveDependency(ResolvedDependency dependency)
        {
            FindChildDependencies(dependency);
            GenerateExpression(dependency);
            GenerateInstanceFactory(dependency);
        }

        private void FindChildDependencies(ResolvedDependency dependency, HashSet<ResolvedDependency>? visitedDependencies = null)
        {
            lock (dependency)
            {
                if (dependency.Children == null)
                {
                    if (visitedDependencies != null && visitedDependencies.Contains(dependency))
                    {
                        var error = new CircularDependencyException(visitedDependencies.Select(x => x.Binding.Expression?.Type).Concat(new[] { dependency.Binding.Expression?.Type }).ToArray());
                        dependency.ResolveError = error;
                        throw error;
                    }
                    if (visitedDependencies == null) visitedDependencies = new HashSet<ResolvedDependency>();
                    visitedDependencies.Add(dependency);

                    Dependency[] dependencies = FindChilds(dependency);
                    foreach (Dependency nestedDependency in dependencies)
                    {
                        FindChildDependencies(nestedDependency.Default, visitedDependencies);
                        visitedDependencies.Remove(nestedDependency.Default);
                    }
                    dependency.Children = dependencies;
                }
            }

            Dependency[] FindChilds(ResolvedDependency parent)
            {
                if (parent.Binding.Expression == null) return new Dependency[0];
                var resolvedDependencies = new List<Dependency>();
                if (parent.Binding.Expression.GetParameterExpressions().Where(x => x.Type != ExpressionGenerator.ScopeParameter.Type)
                    .TryExecute(parameter => { resolvedDependencies.Add(GetDependency(parameter.Type)); }, out IList<Exception> dependencyExceptions))
                {
                    throw new SingularityAggregateException($"Could not find all dependencies for {parent.Binding.BindingMetadata.StringRepresentation()}", dependencyExceptions);
                }

                if (parent.Registration.Decorators.GetParameterExpressions().Where(x => x.Type != ExpressionGenerator.ScopeParameter.Type && x.Type != parent.Registration.DependencyType)
                    .TryExecute(parameter => { resolvedDependencies.Add(GetDependency(parameter.Type)); }, out IList<Exception> decoratorExceptions))
                {
                    throw new SingularityAggregateException($"Could not find all decorator dependencies for {parent.Binding.BindingMetadata.StringRepresentation()}", decoratorExceptions);
                }

                return resolvedDependencies.ToArray();
            }
        }

        private void GenerateExpression(ResolvedDependency dependency)
        {
            lock (dependency)
            {
                if (dependency.ResolveError != null) throw dependency.ResolveError;
                if (dependency.Expression == null)
                {
                    foreach (var nestedDependency in dependency.Children!)
                    {
                        GenerateExpression(nestedDependency.Default);
                    }
                    dependency.Expression = _expressionGenerator.GenerateDependencyExpression(dependency, _containerScope, _settings);
                }
            }
        }

        private void GenerateInstanceFactory(ResolvedDependency dependency)
        {
            lock (dependency)
            {
                if (dependency.ResolveError != null) throw dependency.ResolveError;
                if (dependency.InstanceFactory == null)
                {
                    dependency.InstanceFactory = (Func<Scoped, object>)Expression.Lambda(dependency.Expression, ExpressionGenerator.ScopeParameter).CompileFast();
                }
            }
        }

        private static void CheckChildBindings(IResolverPipeline parentDependencyGraph, Dictionary<Type, Dependency> bindings, object syncRoot)
        {
            lock (syncRoot)
            {
                foreach (Dependency childBinding in bindings.Values)
                {
                    if (parentDependencyGraph.Dependencies.TryGetValue(childBinding.Registration.DependencyType, out Dependency _)
                    )
                    {
                        throw new RegistrationAlreadyExistsException(
                            $"Dependency {childBinding.Registration.DependencyType} was already registered in the parent graph!");
                    }
                    else if (childBinding.Registration.DependencyType.IsGenericType)
                    {
                        if (parentDependencyGraph.Dependencies.TryGetValue(
                            childBinding.Registration.DependencyType.GetGenericTypeDefinition(), out Dependency _))
                        {
                            throw new RegistrationAlreadyExistsException(
                                $"Dependency {childBinding.Registration.DependencyType} was already registered as a open generic in the parent graph!");
                        }
                    }
                }
            }
        }
    }
}
