using System;
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
        public ReadOnlyDictionary<Type, Binding> Bindings { get; }
        public ReadOnlyDictionary<Type, Dependency> Dependencies { get; }

        private static readonly MethodInfo _addMethod = typeof(Scoped).GetRuntimeMethod(nameof(Scoped.Add), new[] { typeof(object) });

        public DependencyGraph(IEnumerable<Binding> bindings, Scoped scope, DependencyGraph? parentDependencyGraph = null)
        {
            Bindings = MergeBindings(bindings, parentDependencyGraph);

            var graph = new Graph<Binding>(Bindings.Values);
            Binding[][] updateOrder = graph.GetUpdateOrder(x => GetDependencies(x, Bindings));

            var dependencies = new Dictionary<Type, Dependency>();

            for (var i = 0; i < updateOrder.Length; i++)
            {
                Binding[] group = updateOrder[i];
                if (group.TryExecute(unresolvedDependency =>
                {
                    if (unresolvedDependency.Expression == null) return;
                    ResolvedDependency resolvedDependency = ResolveDependency(unresolvedDependency.DependencyType, unresolvedDependency, dependencies, scope);
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
            var bindings = childBindingConfig.ToDictionary(
                x => x.DependencyType,
                x => x);

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
                    if (parentBinding.Lifetime == Lifetimes.Singleton)
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
                    if (parentBinding.Lifetime == Lifetimes.Singleton)
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

        private static IEnumerable<Binding> GetDependencies(Binding unresolvedDependency, ReadOnlyDictionary<Type, Binding> unresolvedDependencies)
        {
            if (unresolvedDependency.Expression == null) return Enumerable.Empty<Binding>();

            var resolvedDependencies = new List<Binding>();
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

        private ResolvedDependency ResolveDependency(Type dependencyType, Binding unresolvedDependency, Dictionary<Type, Dependency> resolvedDependencies, Scoped scope)
        {
            Expression expression = GenerateDependencyExpression(dependencyType, unresolvedDependency, resolvedDependencies, scope);

            switch (unresolvedDependency.Lifetime)
            {
                case Transient _:
                    break;
                case Singleton _:
                    Delegate action = Expression.Lambda(expression).Compile();
                    object value = action.DynamicInvoke();
                    expression = Expression.Constant(value, dependencyType);
                    break;
                default:
                    throw new InvalidLifetimeException(unresolvedDependency);
            }

            return new ResolvedDependency(expression, unresolvedDependency.Lifetime);
        }

        private static Expression GenerateDependencyExpression(Type dependencyType, Binding binding, Dictionary<Type, Dependency> resolvedDependencies, Scoped scope)
        {
            Expression expression = binding.Expression is LambdaExpression lambdaExpression ? lambdaExpression.Body : binding.Expression;
            var body = new List<Expression>();
            var parameters = new List<ParameterExpression>();
            parameters.AddRange(expression.GetParameterExpressions());

            ParameterExpression instanceParameter = Expression.Variable(dependencyType, $"{expression.Type} instance");
            body.Add(Expression.Assign(instanceParameter, Expression.Convert(expression, dependencyType)));

            if (binding.OnDeath != null)
            {
                scope.RegisterAction(binding.Expression.Type, binding.OnDeath);
                body.Add(Expression.Call(Expression.Constant(scope), _addMethod, instanceParameter));
            }

            if (binding.Decorators.Count > 0)
            {
                Expression previousDecorator = instanceParameter;
                foreach (DecoratorBinding decorator in binding.Decorators)
                {
                    var visitor = new ReplaceExpressionVisitor(decorator.Expression.GetParameterExpressions().First(x => x.Type == instanceParameter.Type), previousDecorator);
                    Expression decoratorExpression = visitor.Visit(decorator.Expression);
                    parameters.AddRange(decoratorExpression.GetParameterExpressions().Where(parameterExpression => parameterExpression.Type != instanceParameter.Type));
                    previousDecorator = decoratorExpression;
                }
                body.Add(previousDecorator);
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