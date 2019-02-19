using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Singularity.Bindings;
using Singularity.Enums;
using Singularity.Exceptions;

namespace Singularity.Graph
{
    internal sealed class DependencyGraph
    {
        public ReadOnlyDictionary<Type, Binding> Bindings { get; }
        public ReadOnlyDictionary<Type, Dependency> Dependencies { get; }

        public DependencyGraph(IEnumerable<Binding> bindings, IEnumerable<IDependencyExpressionGenerator> dependencyExpressionGenerators, DependencyGraph? parentDependencyGraph = null)
        {
            Bindings = MergeBindings(bindings, parentDependencyGraph);

            Dictionary<Type, UnresolvedDependency> unresolvedDependencies =
                Bindings.Values.Where(x => x.Expression != null).
                ToDictionary(
                    x => x.DependencyType, 
                    x => new UnresolvedDependency(x.BindingMetadata, x.DependencyType, x.Expression!, x.Lifetime, x.Decorators, x.OnDeath));
            var graph = new Graph<UnresolvedDependency>(unresolvedDependencies.Values);
            UnresolvedDependency[][] updateOrder = graph.GetUpdateOrder(x => GetDependencies(x, unresolvedDependencies));

            var dependencies = new Dictionary<Type, Dependency>();

            for (var i = 0; i < updateOrder.Length; i++)
            {
                UnresolvedDependency[] group = updateOrder[i];
                if (group.TryExecute(unresolvedDependency =>
                {
                    ResolvedDependency resolvedDependency = ResolveDependency(unresolvedDependency.DependencyType, unresolvedDependency, dependencyExpressionGenerators, dependencies);
                    dependencies.Add(unresolvedDependency.DependencyType, new Dependency(unresolvedDependency, resolvedDependency));
                }, out IList<Exception> exceptions))
                {
                    throw new SingularityAggregateException($"Exceptions occured while resolving dependencies at {i} deep", exceptions);
                }
            }

            Dependencies = new ReadOnlyDictionary<Type, Dependency>(dependencies);
        }

        private static ReadOnlyDictionary<Type, Binding> MergeBindings(IEnumerable<Binding> childBindingConfig, DependencyGraph? parentDependencyGraph)
        {
            var bindings = new Dictionary<Type, Binding>();
            foreach (Binding binding in childBindingConfig)
            {
                bindings.Add(binding.DependencyType, new Binding(binding.BindingMetadata, binding.DependencyType, binding.Expression, binding.Lifetime, binding.Decorators, binding.OnDeath));
            }

            if (parentDependencyGraph != null)
            {
                MergeParentBindings(parentDependencyGraph, bindings);
            }

            return new ReadOnlyDictionary<Type, Binding>(bindings);
        }

        private static void MergeParentBindings(DependencyGraph parentDependencyGraph, Dictionary<Type, Binding> bindings)
        {
            foreach (Binding parentBinding in parentDependencyGraph.Bindings.Values)
            {
                if (bindings.TryGetValue(parentBinding.DependencyType, out Binding childBinding))
                {
                    List<DecoratorBinding> decorators;
                    Expression? expression;
                    Action<object>? onDeathAction;
                    BindingMetadata bindingMetadata;
                    if (parentBinding.Lifetime == Lifetime.PerContainer)
                    {
                        if (childBinding.Expression == null)
                        {
                            Dependency parentDependency = parentDependencyGraph.Dependencies[childBinding.DependencyType];
                            expression = parentDependency.ResolvedDependency.Expression;
                            bindingMetadata = parentDependency.UnresolvedDependency.BindingMetadata;
                            decorators = childBinding.Decorators.ToList(); //The resolved expression already contains the decorators of the parent
                        }
                        else
                        {
                            bindingMetadata = childBinding.BindingMetadata;
                            expression = childBinding.Expression;
                            decorators = parentBinding.Decorators.Concat(childBinding.Decorators).ToList();
                        }

                        onDeathAction = childBinding.OnDeath;
                    }
                    else
                    {
                        bindingMetadata = parentBinding.BindingMetadata;
                        expression = childBinding.Expression ?? parentBinding.Expression;
                        decorators = parentBinding.Decorators.Concat(childBinding.Decorators).ToList();
                        onDeathAction = parentBinding.OnDeath;
                    }

                    var readonlyBinding = new Binding(bindingMetadata, childBinding.DependencyType, expression, parentBinding.Lifetime, decorators, onDeathAction);
                    bindings[childBinding.DependencyType] = readonlyBinding;
                }
                else
                {
                    if (parentBinding.Lifetime == Lifetime.PerContainer)
                    {
                        var readonlyBinding = new Binding(
                            parentBinding.BindingMetadata,
                            parentBinding.DependencyType,
                            parentBinding.Expression,
                            parentBinding.Lifetime,
                            parentBinding.Decorators,
                            null);
                        bindings.Add(parentBinding.DependencyType, readonlyBinding);
                    }
                    else
                    {
                        bindings.Add(parentBinding.DependencyType, parentBinding);
                    }
                }
            }
        }

        private static IEnumerable<UnresolvedDependency> GetDependencies(UnresolvedDependency unresolvedDependency, Dictionary<Type, UnresolvedDependency> unresolvedDependencies)
        {
            var resolvedDependencies = new List<UnresolvedDependency>();
            if (unresolvedDependency.Expression.GetParameterExpressions()
                .TryExecute(dependencyType => { resolvedDependencies.Add(GetDependency(dependencyType.Type, unresolvedDependencies)); }, out IList<Exception> dependencyExceptions))
            {
                throw new SingularityAggregateException($"Could not find all dependencies for registered binding in {unresolvedDependency.BindingMetadata.GetPosition()}", dependencyExceptions);
            }

            if (unresolvedDependency.Decorators
                .SelectMany(x => x.Expression.GetParameterExpressions())
                .Where(x => x.Type != unresolvedDependency.DependencyType)
                .TryExecute(parameterExpression => { resolvedDependencies.Add(GetDependency(parameterExpression.Type, unresolvedDependencies)); }, out IList<Exception> decoratorExceptions))
            {
                throw new SingularityAggregateException($"Could not find all decorator dependencies for registered binding in {unresolvedDependency.BindingMetadata.GetPosition()}", decoratorExceptions);
            }

            return resolvedDependencies;
        }

        private static TValue GetDependency<TValue>(Type type, IReadOnlyDictionary<Type, TValue> unresolvedDependencies)
        {
            if (unresolvedDependencies.TryGetValue(type, out TValue parent)) return parent;
            throw new DependencyNotFoundException(type);
        }

        private ResolvedDependency ResolveDependency(Type dependencyType, UnresolvedDependency unresolvedDependency, IEnumerable<IDependencyExpressionGenerator> dependencyExpressionGenerators, Dictionary<Type, Dependency> resolvedDependencies)
        {
            Expression expression = GenerateDependencyExpression(dependencyType, unresolvedDependency, dependencyExpressionGenerators, resolvedDependencies);

            switch (unresolvedDependency.Lifetime)
            {
                case Lifetime.PerCall:
                    break;
                case Lifetime.PerContainer:
                    Delegate action = Expression.Lambda(expression).Compile();
                    object value = action.DynamicInvoke();
                    expression = Expression.Constant(value, dependencyType);
                    break;
                default:
                    throw new InvalidLifetimeException(unresolvedDependency);
            }
            return new ResolvedDependency(expression);
        }

        private static Expression GenerateDependencyExpression(Type dependencyType, UnresolvedDependency binding, IEnumerable<IDependencyExpressionGenerator> dependencyExpressionGenerators, Dictionary<Type, Dependency> resolvedDependencies)
        {
            Expression expression = binding.Expression is LambdaExpression lambdaExpression ? lambdaExpression.Body : binding.Expression;
            var body = new List<Expression>();
            var parameters = new List<ParameterExpression>();
            parameters.AddRange(expression.GetParameterExpressions());

            ParameterExpression instanceParameter = Expression.Variable(dependencyType, $"{expression.Type} instance");
            body.Add(Expression.Assign(instanceParameter, Expression.Convert(expression, dependencyType)));
            foreach (IDependencyExpressionGenerator dependencyExpressionGenerator in dependencyExpressionGenerators)
            {
                dependencyExpressionGenerator.Generate(binding, instanceParameter, parameters, body);
            }

            foreach (ParameterExpression unresolvedParameter in parameters)
            {
                Dependency dependency = GetDependency(unresolvedParameter.Type, resolvedDependencies);
                body.Insert(0, Expression.Assign(unresolvedParameter, dependency.ResolvedDependency.Expression));
            }

            if (body.Last().Type == typeof(void)) body.Add(instanceParameter);
            return body.Count == 1 ? expression : Expression.Block(parameters.Concat(new[] { instanceParameter }), body);
        }
    }
}