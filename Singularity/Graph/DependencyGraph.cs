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
		public IBindingConfig BindingConfig { get; }

		private readonly ObjectActionContainer _disposableObjects;

		public DependencyGraph(IBindingConfig bindingConfig, DependencyGraph parentDependencyGraph = null)
		{
			if (parentDependencyGraph != null)
				bindingConfig = new BindingConfig(bindingConfig, parentDependencyGraph);
			BindingConfig = bindingConfig;

			_disposableObjects = new ObjectActionContainer();
			var dependencies = GenerateDependencyNodes(bindingConfig);

			Dependencies = new ReadOnlyDictionary<Type, DependencyNode>(dependencies);

			var graph = new Graph<DependencyNode>(Dependencies.Values);
			var updateOrder = graph.GetUpdateOrder(GetDependencies);

			foreach (var dependencyNodes in updateOrder)
			{
				foreach (var dependencyNode in dependencyNodes)
				{
					dependencyNode.ResolvedDependency = ResolveDependency(dependencyNode.UnresolvedDependency);
				}
			}
		}

		private Dictionary<Type, DependencyNode> GenerateDependencyNodes(IBindingConfig bindingConfig)
		{
			var dependencies = new Dictionary<Type, DependencyNode>();
			foreach (var binding in bindingConfig.Bindings.Values)
			{
				if (binding.ConfiguredBinding == null && binding.Decorators.Count > 0) continue;
				var expression = GenerateDependencyExpression(binding.DependencyType, binding.ConfiguredBinding.Expression, binding.Decorators, binding.ConfiguredBinding.OnDeath);
				var node = new DependencyNode(new UnresolvedDependency(expression, binding.ConfiguredBinding.Lifetime, binding.ConfiguredBinding.OnDeath));
				dependencies.Add(binding.DependencyType, node);
			}

			return dependencies;
		}

		private Expression GenerateDependencyExpression(Type dependencyType, Expression expression, IReadOnlyCollection<IDecoratorBinding> decorators, Action<object> onDeath)
		{
			var body = new List<Expression>();
			var parameters = new List<ParameterExpression>();
			parameters.AddRange(expression.GetParameterExpressions());

			var instanceParameter = Expression.Variable(dependencyType, $"{expression.Type} instance");
			body.Add(Expression.Assign(instanceParameter, Expression.Convert(expression, dependencyType)));
			AppendOnDeathExpression(expression, onDeath, body, instanceParameter);
			AppendDecoratorExpressions(decorators, instanceParameter, parameters, body);

			if (body.Last().Type == typeof(void)) body.Add(instanceParameter);
			return body.Count > 1 ? Expression.Lambda(Expression.Block(parameters.Concat(new[] { instanceParameter }), body), parameters) : expression;
		}

		private void AppendDecoratorExpressions(IReadOnlyCollection<IDecoratorBinding> decorators, ParameterExpression instanceParameter, List<ParameterExpression> parameters, List<Expression> body)
		{
			if (decorators.Count > 0)
			{
				Expression previousDecorator = instanceParameter;
				foreach (var decorator in decorators)
				{
					var visitor = new ReplaceExpressionVisitor(decorator.Expression.GetParameterExpressions().First(x => x.Type == instanceParameter.Type), previousDecorator);
					var decoratorExpression = visitor.Visit(decorator.Expression);
					parameters.AddRange(decoratorExpression.GetParameterExpressions().Where(parameterExpression => parameterExpression.Type != instanceParameter.Type));
					previousDecorator = decoratorExpression;
				}
				body.Add(previousDecorator);
			}
		}

		private void AppendOnDeathExpression(Expression expression, Action<object> onDeath, List<Expression> body, ParameterExpression instanceParameter)
		{
			var addMethodInfo = typeof(ObjectActionContainer).GetRuntimeMethod(nameof(ObjectActionContainer.Add), new[] { typeof(object) });
			if (onDeath != null)
			{
				_disposableObjects.AddAction(expression.Type, onDeath);
				body.Add(Expression.Call(Expression.Constant(_disposableObjects), addMethodInfo, instanceParameter));
			}
		}

		private IEnumerable<DependencyNode> GetDependencies(DependencyNode dependencyNode)
		{
			var parameters = dependencyNode.UnresolvedDependency.Expression.GetParameterExpressions();
			var resolvedDependencies = new List<DependencyNode>();
			var unresolvedDependencies = new List<Type>();
			foreach (var parameterExpression in parameters)
			{
				try
				{
					resolvedDependencies.Add(GetDependency(parameterExpression.Type));
				}
				catch (CannotResolveDependencyException e)
				{
					unresolvedDependencies.Add(e.Type);
				}
			}
			if (unresolvedDependencies.Count > 0) throw new CannotResolveDependenciesException(dependencyNode.UnresolvedDependency.Expression.Type, unresolvedDependencies);

			return resolvedDependencies;
		}

		private DependencyNode GetDependency(Type type)
		{
			if (Dependencies.TryGetValue(type, out var parent))
			{
				return parent;
			}
			throw new CannotResolveDependencyException(type);
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
				case BlockExpression _:
				case ConstantExpression _:
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
				var dependency = GetDependency(unresolvedParameter.Type);
				body.Add(Expression.Assign(unresolvedParameter, dependency.ResolvedDependency.Expression));
			}
			return body;
		}

		public void Dispose()
		{
			_disposableObjects.Invoke();
		}
	}
}