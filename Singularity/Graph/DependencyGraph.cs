using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Exceptions;
using Singularity.Extensions;
using Singularity.Graph;

namespace Singularity
{
    public class ObjectActionContainer
    {
        public Dictionary<Type, (Action<object> action, List<object> objects)> ActionObjectLists { get; } = new Dictionary<Type, (Action<object> action, List<object> objects)>();

        public void AddAction(Type type, Action<object> action)
        {
            ActionObjectLists.Add(type, (action, new List<object>()));
        }

        public void Add(object obj)
        {
            var type = obj.GetType();
            var list = ActionObjectLists[type];
            list.objects.Add(obj);
        }

        public void Invoke()
        {
            foreach (var (action, objects) in ActionObjectLists.Values)
            {
                foreach (var obj in objects)
                {
                    action.Invoke(obj);
                }
            }
        }
    }

    public class DependencyGraph : IDisposable
    {
        public ReadOnlyDictionary<Type, DependencyNode> Dependencies { get; }
        private readonly ObjectActionContainer _disposableObjects;
        private readonly IBindingConfig _bindingConfig;

        public DependencyGraph(IBindingConfig bindingConfig, DependencyGraph parentDependencyGraph = null)
        {
            var mergedBindingConfig = new BindingConfig(bindingConfig, parentDependencyGraph?._bindingConfig);
            _bindingConfig = mergedBindingConfig;

            var decoratorsDic = mergedBindingConfig.Decorators.GroupBy(x => x.DependencyType).ToDictionary(x => x.Key, bindings => bindings.ToArray());

            _disposableObjects = new ObjectActionContainer();
            var dependencies = new Dictionary<Type, DependencyNode>();
            foreach (var binding in mergedBindingConfig.Bindings.Values)
            {
                var expression = GetDependencyExpression(binding, decoratorsDic.TryGetDefaultValue(binding.DependencyType));
                var node = new DependencyNode(new UnresolvedDependency(expression, binding.ConfiguredBinding.Lifetime, binding.ConfiguredBinding.OnDeath));
                dependencies.Add(binding.DependencyType, node);
            }

            Dependencies = new ReadOnlyDictionary<Type, DependencyNode>(dependencies);

            var graph = new Graph<KeyValuePair<Type, DependencyNode>>(Dependencies);
            var updateOrder = graph.GetUpdateOrder(node => node.Value.UnresolvedDependency.Expression.GetParameterExpressions().Select(x => GetDependency(x.Type)));

            foreach (var dependencyNodes in updateOrder)
            {
                foreach (var dependencyNode in dependencyNodes)
                {
                    dependencyNode.Value.ResolvedDependency = ResolveDependency(dependencyNode.Value.UnresolvedDependency);
                }
            }
        }

        private KeyValuePair<Type, DependencyNode> GetDependency(Type type)
        {
            if (Dependencies.TryGetValue(type, out var parent))
            {
                return new KeyValuePair<Type, DependencyNode>(type, parent);
            }
            throw new CannotResolveDependencyException($"Dependency {type} was not registered");
        }

        private Expression GetDependencyExpression(IBinding binding, IDecoratorBinding[] decorators)
        {
            var expression = binding.ConfiguredBinding.Expression;


            var body = new List<Expression>();
            var allParameters = new List<ParameterExpression>();
            allParameters.AddRange(expression.GetParameterExpressions());

            var instanceParameter = Expression.Variable(binding.DependencyType, $"{expression.Type} instance");
            var variables = new List<ParameterExpression> { instanceParameter };

            body.Add(Expression.Assign(instanceParameter, Expression.Convert(expression, binding.DependencyType)));
            var addMethodInfo = typeof(ObjectActionContainer).GetRuntimeMethod(nameof(ObjectActionContainer.Add), new[] { typeof(object) });
            if (binding.ConfiguredBinding.OnDeath != null)
            {
                _disposableObjects.AddAction(expression.Type, binding.ConfiguredBinding.OnDeath);
                body.Add(Expression.Call(Expression.Constant(_disposableObjects), addMethodInfo, instanceParameter));
            }
            if (decorators != null && decorators.Length > 0)
            {
                var previousDecorator = instanceParameter;
                foreach (var decorator in decorators)
                {
                    var decoratorInstance = Expression.Variable(binding.DependencyType, $"{decorator.Expression.Type} instance");
                    variables.Add(decoratorInstance);
                    var visitor = new ReplaceExpressionVisitor(decorator.Expression.GetParameterExpressions().First(x => x.Type == binding.DependencyType), previousDecorator);
                    var decoratorExpression = visitor.Visit(decorator.Expression);
                    foreach (var parameterExpression in decoratorExpression.GetParameterExpressions())
                    {
                        if (parameterExpression.Type != binding.DependencyType)
                            allParameters.Add(parameterExpression);
                    }

                    body.Add(Expression.Assign(decoratorInstance, decoratorExpression));
                    previousDecorator = decoratorInstance;
                }
                return Expression.Lambda(Expression.Block(allParameters.Concat(variables), body), allParameters);
            }
            body.Add(instanceParameter);
            expression = Expression.Lambda(Expression.Block(allParameters.Concat(variables), body), allParameters);
            return expression;
        }

        private ResolvedDependency ResolveDependency(UnresolvedDependency unresolvedDependency)
        {
            var expression = ResolveMethodCallExpression(unresolvedDependency.Expression);

            switch (unresolvedDependency.Lifetime)
            {
                case Lifetime.PerCall:
                    break;
                case Lifetime.PerContainer:
                    var action = Expression.Lambda(expression).Compile();
                    var value = action.DynamicInvoke();
                    expression = Expression.Constant(value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new ResolvedDependency(expression);
        }

        private Expression ResolveMethodCallExpression(Expression expression)
        {
            switch (expression)
            {
                case LambdaExpression lambdaExpression:
                    {
                        var body = ResolveMethodCallParameters(lambdaExpression.Parameters);
                        var blockExpression = (BlockExpression)lambdaExpression.Body;
                        var newBody = Expression.Block(blockExpression.Variables, body.Concat(blockExpression.Expressions));
                        expression = Expression.Block(lambdaExpression.Parameters, newBody);
                    }
                    break;
                case NewExpression newExpression:
                    {
                        if (newExpression.Arguments.Count == 0) break;
                        var body = ResolveMethodCallParameters(newExpression.Arguments);
                        body.Add(newExpression);
                        expression = Expression.Block(newExpression.Arguments.Cast<ParameterExpression>(), body);
                    }
                    break;
                case BlockExpression blockExpression:
                    {
                        expression = blockExpression;
                    }
                    break;
                default:
                    throw new NotSupportedException($"The expression of type {expression.GetType()} is not supported");
            }
            return expression;
        }

        private List<Expression> ResolveMethodCallParameters(IEnumerable<Expression> parameterExpressions)
        {
            var body = new List<Expression>();
            foreach (var unresolvedParameter in parameterExpressions)
            {
                if (Dependencies.TryGetValue(unresolvedParameter.Type, out var dependency))
                {
                    body.Add(Expression.Assign(unresolvedParameter, dependency.ResolvedDependency.Expression));
                }
                else
                {
                    throw new CannotResolveDependencyException($"A instance of type {unresolvedParameter} is needed but was not found in the container.");
                }
            }
            return body;
        }

        public void Dispose()
        {
            _disposableObjects.Invoke();
        }
    }
}