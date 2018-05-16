using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Bindings;
using Singularity.Collections;
using Singularity.Exceptions;
using Singularity.Extensions;

namespace Singularity.Graph
{
	public class DependencyGraph : IDisposable
	{
		public ReadOnlyDictionary<Type, DependencyNode> Dependencies { get; }
		private readonly ObjectActionContainer _disposableObjects;
		public readonly IBindingConfig _bindingConfig;

		public DependencyGraph(IBindingConfig bindingConfig, DependencyGraph parentDependencyGraph = null)
		{
			if (parentDependencyGraph != null)
				bindingConfig = new BindingConfig(bindingConfig, parentDependencyGraph);
			_bindingConfig = bindingConfig;

			_disposableObjects = new ObjectActionContainer();
			var dependencies = new Dictionary<Type, DependencyNode>();
			foreach (var binding in bindingConfig.Bindings.Values)
			{
				if (binding.ConfiguredBinding == null && binding.Decorators.Count > 0) continue;
				var expression = GetDependencyExpression(binding.DependencyType, binding.ConfiguredBinding.Expression, binding.Decorators, binding.ConfiguredBinding.OnDeath);
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

		private Expression GetDependencyExpression(Type dependencyType, Expression expression, IReadOnlyCollection<IDecoratorBinding> decorators, Action<object> onDeath)
		{
			var body = new List<Expression>();
			var allParameters = new List<ParameterExpression>();
			allParameters.AddRange(expression.GetParameterExpressions());

			var instanceParameter = Expression.Variable(dependencyType, $"{expression.Type} instance");
			var variables = new List<ParameterExpression> { instanceParameter };

			body.Add(Expression.Assign(instanceParameter, Expression.Convert(expression, dependencyType)));
			var addMethodInfo = typeof(ObjectActionContainer).GetRuntimeMethod(nameof(ObjectActionContainer.Add), new[] { typeof(object) });
			if (onDeath != null)
			{
				_disposableObjects.AddAction(expression.Type, onDeath);
				body.Add(Expression.Call(Expression.Constant(_disposableObjects), addMethodInfo, instanceParameter));
			}
			if (decorators != null && decorators.Count > 0)
			{
				var previousDecorator = instanceParameter;
				foreach (var decorator in decorators)
				{
					var decoratorInstance = Expression.Variable(dependencyType, $"{decorator.Expression.Type} instance");
					variables.Add(decoratorInstance);
					var visitor = new ReplaceExpressionVisitor(decorator.Expression.GetParameterExpressions().First(x => x.Type == dependencyType), previousDecorator);
					var decoratorExpression = visitor.Visit(decorator.Expression);
					foreach (var parameterExpression in decoratorExpression.GetParameterExpressions())
					{
						if (parameterExpression.Type != dependencyType)
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