using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FastExpressionCompiler;
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
                                                       where m.Name == nameof(CreateEnumerableDependency)
                                                       select m).Single();
        }

        public DependencyGraph(ReadOnlyCollection<Binding> bindings, Scoped scope, DependencyGraph? parentDependencyGraph = null)
        {
            _defaultScope = scope;
            Dependencies = MergeBindings(bindings, parentDependencyGraph);
        }

        public Expression? GetResolvedExpression(Type type)
        {
            Dependency dependency = GetDependency(type);
            ResolveDependency(dependency);
            return dependency.Expression;
        }

        public Func<Scoped, object>? GetResolvedFactory(Type type)
        {
            Dependency dependency = GetDependency(type);
            ResolveDependency(dependency);
            return dependency.InstanceFactory;
        }

        private void ResolveDependency(Dependency dependency)
        {
            FindDependencies(dependency);
            GenerateExpression(dependency);
            GenerateInstanceFactory(dependency);
        }

        private void FindDependencies(Dependency dependency, HashSet<Dependency>? visitedDependencies = null)
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
                    if (visitedDependencies == null) visitedDependencies = new HashSet<Dependency>();
                    visitedDependencies.Add(dependency);

                    Dependency[] dependencies = GetDependencies(dependency);
                    foreach (Dependency nestedDependency in dependencies)
                    {
                        FindDependencies(nestedDependency, visitedDependencies);
                        visitedDependencies.Remove(nestedDependency);
                    }
                    dependency.Children = dependencies;
                }
            }
        }

        private void GenerateExpression(Dependency dependency)
        {
            lock (dependency)
            {
                if (dependency.ResolveError != null) throw dependency.ResolveError;
                if (dependency.Expression == null)
                {
                    foreach (var nestedDependency in dependency.Children)
                    {
                        GenerateExpression(nestedDependency);
                    }
                    dependency.Expression = _expressionGenerator.GenerateDependencyExpression(dependency, _defaultScope);
                }
            }
        }

        private void GenerateInstanceFactory(Dependency dependency)
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
                            if (parentBinding.Expression == null)
                            {
                                parentDependencyGraph.ResolveDependency(parentBinding);
                            }
                            expression = parentBinding.Expression!;
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

        private Dependency[] GetDependencies(Dependency dependency)
        {
            if (dependency.Binding.Expression == null) return new Dependency[0];
            var resolvedDependencies = new List<Dependency>();
            if (dependency.Binding.Parameters
                .TryExecute(dependencyType => { resolvedDependencies.Add(GetDependency(dependencyType.Type)); }, out IList<Exception> dependencyExceptions))
            {
                throw new SingularityAggregateException($"Could not find all dependencies for {dependency.Binding.BindingMetadata.StringRepresentation()}", dependencyExceptions);
            }

            if (dependency.Binding.DecoratorParameters
                .TryExecute(parameterExpression => { resolvedDependencies.Add(GetDependency(parameterExpression.Type)); }, out IList<Exception> decoratorExceptions))
            {
                throw new SingularityAggregateException($"Could not find all decorator dependencies for {dependency.Binding.BindingMetadata.StringRepresentation()}", decoratorExceptions);
            }

            return resolvedDependencies.ToArray();
        }

        private Dependency GetDependency(Type type)
        {
            return GetDependencyCollection(type).Array[0];
        }

        private ArrayList<Dependency>? TryGetDependencyCollection(Type type)
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
                        ArrayList<Dependency> dependencies = TryGetDependencyCollection(type.GenericTypeArguments[0]) ?? ArrayList<Dependency>.Empty;
                        foreach (Dependency dependency in dependencies.Array)
                        {
                            ResolveDependency(dependency);
                        }
                        IEnumerable<Func<Scoped, object>?> instanceFactories = dependencies.Array.Select(x => x.InstanceFactory!).ToArray();

                        MethodInfo method = GenericCreateEnumerableExpressionMethod.MakeGenericMethod(type.GenericTypeArguments);
                        var enumerableDependency = (ArrayList<Dependency>)method.Invoke(this, new object[] { type, instanceFactories });

                        return enumerableDependency;
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
            }

            return null;
        }

        private ArrayList<Dependency> GetDependencyCollection(Type type)
        {
            ArrayList<Dependency>? dependency = TryGetDependencyCollection(type);
            if (dependency == null) throw new DependencyNotFoundException(type);
            return dependency;
        }

        private ArrayList<Dependency> CreateEnumerableDependency<T>(Type type, Func<Scoped, object>[] instanceFactories)
        {
            IEnumerable<T> enumerable = instanceFactories.Length == 0 ? new T[0] : CreateEnumerable<T>(instanceFactories);
            ConstantExpression expression = Expression.Constant(enumerable);
            ArrayList<Dependency> dependency = AddDependency(type, expression, CreationMode.Transient, scope => enumerable);
            return dependency;
        }

        private IEnumerable<T> CreateEnumerable<T>(Func<Scoped, object>[] instanceFactories)
        {
            foreach (Func<Scoped, object> instanceFactory in instanceFactories)
            {
                yield return (T)instanceFactory.Invoke(_defaultScope);
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

        private ArrayList<Dependency> AddDependency(Type type, Expression expression, CreationMode creationMode, Func<Scoped, object> instanceFactory = null)
        {
            var binding = new Binding(new BindingMetadata(type), type, expression, creationMode, new Expression[0], null);
            var dependency = new ArrayList<Dependency>(new Dependency(binding));
            if (instanceFactory != null)
            {
                dependency.Array[0].Expression = expression;
                dependency.Array[0].InstanceFactory = instanceFactory;
            }
            Dependencies.Add(type, dependency);
            return dependency;
        }
    }
}