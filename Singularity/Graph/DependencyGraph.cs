using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Singularity.Exceptions;
using Singularity.Extensions;
using Singularity.Graph;

namespace Singularity
{
    public class DependencyGraph
    {
        public ReadOnlyDictionary<Type, DependencyNode> Dependencies { get; }

        public DependencyGraph(IBindingConfig bindingConfig, DependencyGraph parentGraph = null)
        {
	        var decoratorsDic = bindingConfig.Decorators.GroupBy(x => x.DependencyType).ToDictionary(x => x.Key, bindings => bindings.ToArray());

	        var dependencies = new Dictionary<Type, DependencyNode>();
			foreach (var binding in bindingConfig.Bindings.Values)
            {
                var expression = GetDependencyExpression(binding, decoratorsDic.TryGetDefaultValue(binding.DependencyType));
                var node = new DependencyNode(expression, binding.ConfiguredBinding.Lifetime);
                dependencies.Add(binding.DependencyType, node);
            }

	        if (parentGraph != null)
	        {
		        foreach (var parentGraphDependency in parentGraph.Dependencies)
		        {
			        if(!dependencies.ContainsKey(parentGraphDependency.Key)) dependencies.Add(parentGraphDependency.Key, parentGraphDependency.Value);
		        }
	        }
			Dependencies = new ReadOnlyDictionary<Type, DependencyNode>(dependencies);

            var graph = new Graph<DependencyNode>(Dependencies.Values);
            var updateOrder = graph.GetUpdateOrder(node => node.Expression.GetParameterExpressions().Select(x => GetDependency(x.Type)));

            foreach (var dependencyNodes in updateOrder)
            {
                foreach (var dependencyNode in dependencyNodes)
                {
                    ResolveDependency(dependencyNode);
                }
            }
        }

        private DependencyNode GetDependency(Type type)
        {
            if (Dependencies.TryGetValue(type, out var parent))
            {
                return parent;
            }
            throw new CannotResolveDependencyException($"Dependency {type} was not registered");
        }

        private Expression GetDependencyExpression(IBinding binding, IDecoratorBinding[] decorators)
        {
            var expression = binding.ConfiguredBinding.Expression;

            if (decorators != null && decorators.Length > 0)
            {
                var body = new List<Expression>();
                var allParameters = new List<ParameterExpression>();
                allParameters.AddRange(expression.GetParameterExpressions());

                var instanceParameter = Expression.Variable(binding.DependencyType, $"{expression.Type} instance");
                var variables = new List<ParameterExpression> { instanceParameter };

                body.Add(Expression.Assign(instanceParameter, Expression.Convert(expression, binding.DependencyType)));
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

                expression = Expression.Lambda(Expression.Block(allParameters.Concat(variables), body), allParameters);
            }
            return expression;
        }

        private void ResolveDependency(DependencyNode dependencyNode)
        {
            var expression = ResolveMethodCallExpression(dependencyNode.Expression);

            switch (dependencyNode.Lifetime)
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
            dependencyNode.Expression = expression;
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
                    body.Add(Expression.Assign(unresolvedParameter, dependency.Expression));
                }
                else
                {
                    throw new CannotResolveDependencyException($"A instance of type {unresolvedParameter} is needed but was not found in the container.");
                }
            }
            return body;
        }
    }
}