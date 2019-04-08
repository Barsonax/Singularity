using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FastExpressionCompiler;
using Singularity.Bindings;
using Singularity.Exceptions;
using Singularity.Expressions;

namespace Singularity.Graph
{
    internal sealed class DependencyGraph
    {
        private Dictionary<Type, Dependency> Dependencies { get; }
        private readonly Scoped _defaultScope;
        private readonly ExpressionGenerator _expressionGenerator = new ExpressionGenerator();
        private readonly object _syncRoot;
        private static readonly MethodInfo GenericCreateEnumerableExpressionMethod;
        private readonly DependencyGraph? _parentGraph;

        static DependencyGraph()
        {
            GenericCreateEnumerableExpressionMethod = (from m in typeof(DependencyGraph).GetRuntimeMethods()
                                                       where m.Name == nameof(CreateEnumerableDependency)
                                                       select m).Single();
        }

        public DependencyGraph(ReadOnlyCollection<ReadonlyRegistration> bindings, Scoped scope, DependencyGraph? parentDependencyGraph = null)
        {
            _parentGraph = parentDependencyGraph;
            _syncRoot = parentDependencyGraph?._syncRoot ?? new object();
            _defaultScope = scope;
            Dependencies = MergeBindings(bindings, parentDependencyGraph);
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

        private void ResolveDependency(ResolvedDependency dependency)
        {
            FindDependencies(dependency);
            GenerateExpression(dependency);
            GenerateInstanceFactory(dependency);
        }

        private void FindDependencies(ResolvedDependency dependency, HashSet<ResolvedDependency>? visitedDependencies = null)
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

                    Dependency[] dependencies = GetDependencies(dependency);
                    foreach (Dependency nestedDependency in dependencies)
                    {
                        FindDependencies(nestedDependency.Default, visitedDependencies);
                        visitedDependencies.Remove(nestedDependency.Default);
                    }
                    dependency.Children = dependencies;
                }
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

        private static Dictionary<Type, Dependency> MergeBindings(ReadOnlyCollection<ReadonlyRegistration> registrations, DependencyGraph? parentDependencyGraph)
        {
            var dependencies = new Dictionary<Type, Dependency>(registrations.Count);
            foreach (ReadonlyRegistration registration in registrations)
            {
                dependencies.Add(registration.DependencyType, new Dependency(registration));
            }
            if (parentDependencyGraph != null)
            {
                CheckChildBindings(parentDependencyGraph, dependencies);
            }

            return dependencies;
        }

        private static void CheckChildBindings(DependencyGraph parentDependencyGraph, Dictionary<Type, Dependency> bindings)
        {
            foreach (Dependency childBinding in bindings.Values)
            {
                if (parentDependencyGraph.Dependencies.TryGetValue(childBinding.Registration.DependencyType, out Dependency _))
                {
                    throw new RegistrationAlreadyExistsException($"Dependency {childBinding.Registration.DependencyType} was already registered in the parent graph!");
                }
                else if (childBinding.Registration.DependencyType.IsGenericType)
                {
                    if (parentDependencyGraph.Dependencies.TryGetValue(childBinding.Registration.DependencyType.GetGenericTypeDefinition(), out Dependency _))
                    {
                        throw new RegistrationAlreadyExistsException($"Dependency {childBinding.Registration.DependencyType} was already registered as a open generic in the parent graph!");
                    }
                }
            }
        }

        private Dependency[] GetDependencies(ResolvedDependency dependency)
        {
            if (dependency.Binding.Expression == null) return new Dependency[0];
            var resolvedDependencies = new List<Dependency>();
            if (dependency.Binding.Expression.GetParameterExpressions()
                .TryExecute(parameter => { resolvedDependencies.Add(GetDependency(parameter.Type)); }, out IList<Exception> dependencyExceptions))
            {
                throw new SingularityAggregateException($"Could not find all dependencies for {dependency.Binding.BindingMetadata.StringRepresentation()}", dependencyExceptions);
            }

            if (dependency.Registration.Decorators.GetParameterExpressions().Where(x => x.Type != dependency.Registration.DependencyType)
                .TryExecute(parameter => { resolvedDependencies.Add(GetDependency(parameter.Type)); }, out IList<Exception> decoratorExceptions))
            {
                throw new SingularityAggregateException($"Could not find all decorator dependencies for {dependency.Binding.BindingMetadata.StringRepresentation()}", decoratorExceptions);
            }

            return resolvedDependencies.ToArray();
        }

        private Dependency? TryGetDependency(Type type)
        {
            lock (_syncRoot)
            {
                if (Dependencies.TryGetValue(type, out Dependency parent)) return parent;

                if (!type.IsInterface)
                {
                    return GetOrCreateDependency(type);
                }

                if (type.IsGenericType)
                {
                    if (typeof(IEnumerable).IsAssignableFrom(type))
                    {
                        IEnumerable<ResolvedDependency> dependencies = TryGetDependency(type.GenericTypeArguments[0])?.ResolvedDependencies.Array ?? new ResolvedDependency[0];
                        foreach (ResolvedDependency dependency in dependencies)
                        {
                            ResolveDependency(dependency);
                        }
                        IEnumerable<Func<Scoped, object>?> instanceFactories = dependencies.Select(x => x.InstanceFactory!).ToArray();

                        MethodInfo method = GenericCreateEnumerableExpressionMethod.MakeGenericMethod(type.GenericTypeArguments);
                        var enumerableDependency = (Dependency)method.Invoke(this, new object[] { type, instanceFactories });

                        return enumerableDependency;
                    }
                    Type genericTypeDefinition = type.GetGenericTypeDefinition();
                    if (Dependencies.TryGetValue(genericTypeDefinition, out Dependency openGenericDependencyCollection))
                    {
                        ResolvedDependency openGenericDependency = openGenericDependencyCollection.Default;
                        Type openGenericType = ((OpenGenericTypeExpression)openGenericDependency.Binding.Expression!).OpenGenericType;
                        Type closedGenericType = openGenericType.MakeGenericType(type.GenericTypeArguments);
                        Expression newExpression = closedGenericType.AutoResolveConstructorExpression();
                        return AddDependency(type, newExpression, openGenericDependency.Binding.CreationMode);
                    }
                }
                return _parentGraph?.TryGetDependency(type);
            }
        }

        private Dependency GetDependency(Type type)
        {
            Dependency? dependency = TryGetDependency(type);
            if (dependency == null) throw new DependencyNotFoundException(type);
            return dependency;
        }

        private Dependency CreateEnumerableDependency<T>(Type type, Func<Scoped, object>[] instanceFactories)
        {
            IEnumerable<T> enumerable = instanceFactories.Length == 0 ? new T[0] : CreateEnumerable<T>(instanceFactories);
            ConstantExpression expression = Expression.Constant(enumerable);
            Dependency dependency = AddDependency(type, expression, CreationMode.Transient, scope => enumerable);
            return dependency;
        }

        private IEnumerable<T> CreateEnumerable<T>(Func<Scoped, object>[] instanceFactories)
        {
            foreach (Func<Scoped, object> instanceFactory in instanceFactories)
            {
                yield return (T)instanceFactory.Invoke(_defaultScope);
            }
        }

        private Dependency GetOrCreateDependency(Type type)
        {
            return GetOrCreateDependency(type, type.AutoResolveConstructorExpression(), CreationMode.Transient);
        }

        private Dependency GetOrCreateDependency(Type type, Expression expression, CreationMode creationMode)
        {
            if (!Dependencies.TryGetValue(type, out Dependency dependency))
            {
                dependency = AddDependency(type, expression, creationMode);
            }

            return dependency;
        }

        private Dependency AddDependency(Type type, Expression expression, CreationMode creationMode, Func<Scoped, object> instanceFactory = null)
        {
            var binding = new Binding(new BindingMetadata(type), expression, creationMode, null);

            var registration = new ReadonlyRegistration(type, binding, new Expression[0]);
            var dependency = new Dependency(registration);
            if (instanceFactory != null)
            {
                dependency.Default.Expression = expression;
                dependency.Default.InstanceFactory = instanceFactory;
            }
            Dependencies.Add(type, dependency);
            return dependency;
        }
    }
}