using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Collections;
using Singularity.Exceptions;
using Singularity.Expressions;

namespace Singularity.Graph
{
    internal sealed class DependencyGraph
    {
        private Dictionary<Type, ArrayList<Dependency>> Dependencies { get; }
        private readonly Scoped _defaultScope;
        private readonly ExpressionGenerator _expressionGenerator = new ExpressionGenerator();
        private readonly object _locker = new object();
        private static readonly MethodInfo GenericCreateEnumerableExpressionMethod;

        static DependencyGraph()
        {
            GenericCreateEnumerableExpressionMethod = (from m in typeof(DependencyGraph).GetRuntimeMethods()
                                                       where m.Name == nameof(CreateEnumerableExpression)
                                                       select m).Single();
        }

        public DependencyGraph(ReadOnlyCollection<Binding> bindings, Scoped scope, DependencyGraph? parentDependencyGraph = null)
        {
            _defaultScope = scope;
            Dependencies = MergeBindings(bindings, parentDependencyGraph);
        }

        public ResolvedDependency? GetResolvedDependency(Type type, bool throwError = true)
        {
            Dependency? dependency = GetDependency(type, throwError);
            if (dependency == null && !throwError) return null;
            ResolveDependency(dependency);
            return dependency!.ResolvedDependency!;
        }

        private void ResolveDependency(Dependency dependency)
        {
            FindDependencies(dependency);
            GenerateInstanceFactory(dependency);
        }

        private void FindDependencies(Dependency dependency, HashSet<Dependency> visitedDependencies = null)
        {
            lock (dependency)
            {
                if (visitedDependencies != null && visitedDependencies.Contains(dependency))
                {
                    var error = new CircularDependencyException(visitedDependencies.Select(x => x.Binding.Expression.Type).Concat(new[] { dependency.Binding.Expression.Type }).ToArray());
                    dependency.ResolveError = error;
                    throw error;
                }
                if (dependency.Dependencies == null)
                {
                    if (visitedDependencies == null) visitedDependencies = new HashSet<Dependency>();
                    visitedDependencies.Add(dependency);


                    foreach (var nestedDependency in GetDependencies(dependency))
                    {
                        FindDependencies(nestedDependency, visitedDependencies);
                        visitedDependencies.Remove(nestedDependency);
                    }
                    dependency.Dependencies = GetDependencies(dependency);
                }
            }
        }

        private void GenerateInstanceFactory(Dependency dependency)
        {
            lock (dependency)
            {
                if (dependency.ResolveError != null) throw dependency.ResolveError;
                if (dependency.ResolvedDependency == null)
                {
                    foreach (var nestedDependency in dependency.Dependencies)
                    {
                        GenerateInstanceFactory(nestedDependency);
                    }
                    Expression expression = _expressionGenerator.GenerateDependencyExpression(dependency, _defaultScope);
                    var instanceFactory = (Func<object>)Expression.Lambda(expression).Compile();
                    dependency.ResolvedDependency = new ResolvedDependency(expression, instanceFactory);
                }
            }
        }

        private static Dictionary<Type, ArrayList<Dependency>> MergeBindings(ReadOnlyCollection<Binding> bindings, DependencyGraph? parentDependencyGraph)
        {
            var dependencies = new Dictionary<Type, ArrayList<Dependency>>(bindings.Count);
            foreach (Binding binding in bindings)
            {
                if (!dependencies.TryGetValue(binding.DependencyType, out ArrayList<Dependency> dependency))
                {
                    dependency = new ArrayList<Dependency>(new Dependency(binding));
                    dependencies.Add(binding.DependencyType, dependency);
                }
                else
                {
                    dependency.Add(new Dependency(binding));
                }
            }
            if (parentDependencyGraph != null)
            {
                MergeParentBindings(parentDependencyGraph, dependencies);
            }

            return dependencies;
        }

        private static void MergeParentBindings(DependencyGraph parentDependencyGraph, Dictionary<Type, ArrayList<Dependency>> bindings)
        {
            foreach (ArrayList<Dependency> parentBindingCollection in parentDependencyGraph.Dependencies.Values)
            {
                Dependency parentBinding = parentBindingCollection.Array[0];
                if (bindings.TryGetValue(parentBinding.Binding.DependencyType, out ArrayList<Dependency> childBindingCollection))
                {
                    Dependency childBinding = childBindingCollection.Array[0];
                    Expression[] decorators;
                    Expression? expression;
                    Action<object>? onDeathAction;
                    BindingMetadata bindingMetadata;
                    if (parentBinding.Binding.CreationMode == CreationMode.Singleton)
                    {
                        if (childBinding.Binding.Expression == null)
                        {
                            if (parentBinding.ResolvedDependency == null)
                            {
                                parentDependencyGraph.ResolveDependency(parentBinding);
                            }
                            expression = parentBinding.ResolvedDependency!.Expression;
                            bindingMetadata = parentBinding.Binding.BindingMetadata;
                            decorators = childBinding.Binding.Decorators; //The resolved expression already contains the decorators of the parent
                        }
                        else
                        {
                            bindingMetadata = childBinding.Binding.BindingMetadata;
                            expression = childBinding.Binding.Expression;
                            decorators = parentBinding.Binding.Decorators.Concat(childBinding.Binding.Decorators).ToArray();
                        }

                        onDeathAction = childBinding.Binding.OnDeathAction;
                    }
                    else
                    {
                        bindingMetadata = parentBinding.Binding.BindingMetadata;
                        expression = childBinding.Binding.Expression ?? parentBinding.Binding.Expression;
                        decorators = parentBinding.Binding.Decorators.Concat(childBinding.Binding.Decorators).ToArray();
                        onDeathAction = parentBinding.Binding.OnDeathAction;
                    }

                    var readonlyBinding = new Binding(bindingMetadata, childBinding.Binding.DependencyType, expression, parentBinding.Binding.CreationMode, decorators, onDeathAction);
                    bindings[childBinding.Binding.DependencyType] = new ArrayList<Dependency>(new Dependency(readonlyBinding));
                }
                else
                {
                    if (parentBinding.Binding.CreationMode == CreationMode.Singleton)
                    {
                        var readonlyBinding = new Binding(
                            parentBinding.Binding.BindingMetadata,
                            parentBinding.Binding.DependencyType,
                            parentBinding.Binding.Expression,
                            parentBinding.Binding.CreationMode,
                            parentBinding.Binding.Decorators,
                            null);
                        bindings.Add(parentBinding.Binding.DependencyType, new ArrayList<Dependency>(new Dependency(readonlyBinding)));
                    }
                    else
                    {
                        bindings.Add(parentBinding.Binding.DependencyType, new ArrayList<Dependency>(new Dependency(parentBinding.Binding)));
                    }
                }
            }
        }

        private Dependency[] GetDependencies(Dependency unresolvedDependency)
        {
            if (unresolvedDependency.Binding.Expression == null) return new Dependency[0];

            return GetDependencies(unresolvedDependency.GetParameters(), unresolvedDependency.GetDecoratorParameters(), unresolvedDependency.Binding.BindingMetadata);
        }

        private Dependency[] GetDependencies(IEnumerable<ParameterExpression> parameters, IEnumerable<ParameterExpression> decoratorParameters, BindingMetadata bindingMetadata)
        {
            var resolvedDependencies = new List<Dependency>();
            if (parameters
                .TryExecute(dependencyType => { resolvedDependencies.Add(GetDependency(dependencyType.Type)); }, out IList<Exception> dependencyExceptions))
            {
                throw new SingularityAggregateException($"Could not find all dependencies for {bindingMetadata.StringRepresentation()}", dependencyExceptions);
            }

            if (decoratorParameters
                .TryExecute(parameterExpression => { resolvedDependencies.Add(GetDependency(parameterExpression.Type)); }, out IList<Exception> decoratorExceptions))
            {
                throw new SingularityAggregateException($"Could not find all decorator dependencies for {bindingMetadata.StringRepresentation()}", decoratorExceptions);
            }

            return resolvedDependencies.ToArray();
        }

        private Dependency GetDependency(Type type, bool throwError = true)
        {
            return GetDependencyCollection(type, throwError).Array[0];
        }

        private ArrayList<Dependency>? GetDependencyCollection(Type type, bool throwError = true)
        {
            lock (_locker)
            {
                if (Dependencies.TryGetValue(type, out ArrayList<Dependency> parent)) return parent;

                if (!type.IsInterface)
                {
                    return GetOrCreateDependency(type);
                }

                if (type.IsGenericType)
                {
                    if (typeof(IEnumerable).IsAssignableFrom(type))
                    {
                        ArrayList<Dependency>? dependencies = GetDependencyCollection(type.GenericTypeArguments[0], throwError);
                        foreach (var dependency in dependencies.Array)
                        {
                            ResolveDependency(dependency);
                        }
                        var instanceFactories = dependencies.Array.Select(x => x.ResolvedDependency!.InstanceFactory).ToArray();

                        MethodInfo method = GenericCreateEnumerableExpressionMethod.MakeGenericMethod(type.GenericTypeArguments);
                        var expression = (Expression)method.Invoke(this, new object[] { instanceFactories });

                        return AddDependency(type, expression, CreationMode.Transient);
                    }
                    Type genericTypeDefinition = type.GetGenericTypeDefinition();
                    if (Dependencies.TryGetValue(genericTypeDefinition, out ArrayList<Dependency> openGenericDependencyCollection))
                    {
                        Dependency openGenericDependency = openGenericDependencyCollection.Array[0];
                        Type openGenericType = ((OpenGenericTypeExpression)openGenericDependency.Binding.Expression!).OpenGenericType;
                        Type closedGenericType = openGenericType.MakeGenericType(type.GenericTypeArguments);
                        Expression newExpression = closedGenericType.AutoResolveConstructorExpression();
                        return AddDependency(type, newExpression, openGenericDependency.Binding.CreationMode);
                    }
                }

                if (throwError)
                    throw new DependencyNotFoundException(type);
                else return null;
            }
        }

        private Expression CreateEnumerableExpression<T>(Func<object>[] instanceFactories)
        {
            IEnumerable<T> enumerable = CreateEnumerable<T>(instanceFactories);
            return Expression.Constant(enumerable);
        }

        private IEnumerable<T> CreateEnumerable<T>(Func<object>[] instanceFactories)
        {
            foreach (Func<object> instanceFactory in instanceFactories)
            {
                yield return (T)instanceFactory.Invoke();
            }
        }

        private ArrayList<Dependency> GetOrCreateDependency(Type type)
        {
            return GetOrCreateDependency(type, type.AutoResolveConstructorExpression(), CreationMode.Transient);
        }

        private ArrayList<Dependency> GetOrCreateDependency(Type type, Expression expression, CreationMode creationMode)
        {
            if (!Dependencies.TryGetValue(type, out ArrayList<Dependency> dependency))
            {
                dependency = AddDependency(type, expression, creationMode);
            }

            return dependency;
        }

        private ArrayList<Dependency> AddDependency(Type type, Expression expression, CreationMode creationMode)
        {
            var binding = new Binding(new BindingMetadata(type), type, expression, creationMode, new Expression[0], null);
            var dependency = new ArrayList<Dependency>(new Dependency(binding));
            Dependencies.Add(type, dependency);
            return dependency;
        }
    }
}