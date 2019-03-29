using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Singularity.Exceptions;
using Singularity.Expressions;

namespace Singularity.Graph
{
    internal sealed class DependencyGraph
    {
        private Dictionary<Type, Dependency> Dependencies { get; }
        private readonly Scoped _defaultScope;
        private readonly ExpressionGenerator _expressionGenerator = new ExpressionGenerator();
        private readonly object _locker = new object();

        public DependencyGraph(ReadOnlyCollection<Binding> bindings, Scoped scope, DependencyGraph? parentDependencyGraph = null)
        {
            _defaultScope = scope;
            Dependencies = MergeBindings(bindings, parentDependencyGraph);
        }

        public Expression? GetExpression(Type type, bool throwError = true)
        {
            Dependency? dependency = GetDependency(type, throwError);
            if (dependency == null && !throwError) return Expression.Default(type);
            ResolveDependency(dependency);
            return dependency!.ResolvedDependency!.Expression;
        }

        private void ResolveDependency(Dependency dependency)
        {
            lock (dependency)
            {
                if (dependency.Dependencies == null)
                {
                    dependency.Dependencies = GetDependencies(dependency);

                    foreach (var nestedDependency in dependency.Dependencies)
                    {
                        ResolveDependency(nestedDependency);
                    }
                }

                if (dependency.ResolvedDependency == null)
                {
                    Expression expression = _expressionGenerator.GenerateDependencyExpression(dependency, _defaultScope);
                    dependency.ResolvedDependency = new ResolvedDependency(expression);
                }
            }
        }

        private static Dictionary<Type, Dependency> MergeBindings(ReadOnlyCollection<Binding> bindings, DependencyGraph? parentDependencyGraph)
        {
            var dependencies = new Dictionary<Type, Dependency>(bindings.Count);
            foreach (Binding binding in bindings)
            {
                dependencies.Add(binding.DependencyType, new Dependency(binding));
            }
            if (parentDependencyGraph != null)
            {
                MergeParentBindings(parentDependencyGraph, dependencies);
            }

            return dependencies;
        }

        private static void MergeParentBindings(DependencyGraph parentDependencyGraph, Dictionary<Type, Dependency> bindings)
        {
            foreach (Dependency parentBinding in parentDependencyGraph.Dependencies.Values)
            {
                if (bindings.TryGetValue(parentBinding.Binding.DependencyType, out Dependency childBinding))
                {
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
                    bindings[childBinding.Binding.DependencyType] = new Dependency(readonlyBinding);
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
                        bindings.Add(parentBinding.Binding.DependencyType, new Dependency(readonlyBinding));
                    }
                    else
                    {
                        bindings.Add(parentBinding.Binding.DependencyType, new Dependency(parentBinding.Binding));
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
                throw new SingularityAggregateException($"Could not find all dependencies for registered binding in {bindingMetadata.GetPosition()}", dependencyExceptions);
            }

            if (decoratorParameters
                .TryExecute(parameterExpression => { resolvedDependencies.Add(GetDependency(parameterExpression.Type)); }, out IList<Exception> decoratorExceptions))
            {
                throw new SingularityAggregateException($"Could not find all decorator dependencies for registered binding in {bindingMetadata.GetPosition()}", decoratorExceptions);
            }

            return resolvedDependencies.ToArray();
        }

        private Dependency? GetDependency(Type type, bool throwError = true)
        {
            lock (_locker)
            {
                if (Dependencies.TryGetValue(type, out Dependency parent)) return parent;

                if (!type.IsInterface)
                {
                    return GetOrCreateDependency(type);
                }

                if (type.IsGenericType)
                {
                    Type genericTypeDefinition = type.GetGenericTypeDefinition();
                    if (Dependencies.TryGetValue(genericTypeDefinition, out Dependency openGenericDependency))
                    {
                        Type openGenericType = ((OpenGenericTypeExpression)openGenericDependency.Binding.Expression).OpenGenericType;
                        Type closedGenericType = openGenericType.MakeGenericType(type.GenericTypeArguments);
                        NewExpression newExpression = closedGenericType.AutoResolveConstructorExpression();
                        return AddDependency(type, newExpression, openGenericDependency.Binding.CreationMode);
                    }
                }

                if (throwError)
                    throw new DependencyNotFoundException(type);
                else return null;
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

        private Dependency AddDependency(Type type, Expression expression, CreationMode creationMode)
        {
            var binding = new Binding(BindingMetadata.Empty, type, expression, creationMode, new Expression[0], null);
            var dependency = new Dependency(binding);
            Dependencies.Add(type ,dependency);
            return dependency;
        }
    }
}