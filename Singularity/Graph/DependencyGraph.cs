using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using FastExpressionCompiler;
using Singularity.Bindings;
using Singularity.Exceptions;
using Singularity.Expressions;
using Singularity.Graph.Resolvers;

namespace Singularity.Graph
{
    internal sealed class DependencyGraph
    {
        private Dictionary<Type, Dependency> Dependencies { get; }
        private readonly IDependencyResolver[] _resolvers = { new EnumerableDependencyResolver(), new LazyDependencyResolver(), new FactoryDependencyResolver(),  new ConcreteDependencyResolver(), new OpenGenericResolver() };
        private readonly Scoped _defaultScope;
        private readonly ExpressionGenerator _expressionGenerator = new ExpressionGenerator();
        private readonly object _syncRoot;
        private readonly DependencyGraph? _parentGraph;

        public DependencyGraph(ReadOnlyCollection<ReadonlyRegistration> registrations, Scoped scope, DependencyGraph? parentDependencyGraph = null)
        {
            _parentGraph = parentDependencyGraph;
            _syncRoot = parentDependencyGraph?._syncRoot ?? new object();
            _defaultScope = scope;
            var dependencies = new Dictionary<Type, Dependency>(registrations.Count);
            foreach (ReadonlyRegistration registration in registrations)
            {
                dependencies.Add(registration.DependencyType, new Dependency(registration));
            }
            if (parentDependencyGraph != null)
            {
                CheckChildBindings(parentDependencyGraph, dependencies);
            }

            Dependencies = dependencies;
        }

        private static void CheckChildBindings(DependencyGraph parentDependencyGraph, Dictionary<Type, Dependency> bindings)
        {
            lock (parentDependencyGraph._syncRoot)
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

        public Expression? GetResolvedExpression(Type type)
        {
            ResolvedDependency dependency = GetDependency(type).Default;
            ResolveDependency(dependency);
            return dependency.Expression;
        }

        public Func<Scoped, object>? GetResolvedFactory(Type type)
        {
            ResolvedDependency dependency = GetDependency(type).Default;
            ResolveDependency(dependency);
            return dependency.InstanceFactory;
        }

        internal void ResolveDependency(ResolvedDependency dependency)
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
                    foreach (var nestedDependency in dependency.Children)
                    {
                        GenerateExpression(nestedDependency.Default);
                    }
                    dependency.Expression = _expressionGenerator.GenerateDependencyExpression(dependency, _defaultScope);
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

        internal Dependency? TryGetDependency(Type type)
        {
            lock (_syncRoot)
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

                return _parentGraph?.TryGetDependency(type);
            }
        }

        internal Dependency GetDependency(Type type)
        {
            Dependency? dependency = TryGetDependency(type);
            if (dependency == null) throw new DependencyNotFoundException(type);
            return dependency;
        }
    }
}