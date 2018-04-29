using Singularity.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace Singularity
{
	public class BindingBuilder : IDisposable
	{
		public event EventHandler<IEnumerable<IBinding>> OnFinishBuildingDependencies;
		private Container _container;
		private Dictionary<Type, IBinding> _bindings = new Dictionary<Type, IBinding>();

		public BindingBuilder(Container container)
		{
			_container = container;
		}

		public void Dispose()
		{
			ResolveDependencies();
			OnFinishBuildingDependencies.Invoke(this, _bindings.Values);
		}

		private void ResolveDependencies()
		{
			foreach (var dependency in _bindings.Values)
			{
				ResolveDependency(dependency);
			}
		}

		private void ResolveDependency(IBinding binding)
		{
			if (binding.IsResolved) return;
			Expression expression;
			switch (binding.BindingExpression)
			{
				case LambdaExpression lambdaExpression:
					{
						var body = ResolveMethodCallExpression(binding, lambdaExpression.Parameters);
						body.Add(lambdaExpression.Body);
						expression = Expression.Block(lambdaExpression.Parameters, body);
					}
					break;
				case NewExpression newExpression:
					{						
						var body = ResolveMethodCallExpression(binding, newExpression.Arguments);
						body.Add(newExpression);
						expression = Expression.Block(newExpression.Arguments.Cast<ParameterExpression>(), body);
					}
					break;
				default:
					throw new NotSupportedException($"The expression of type {binding.BindingExpression.GetType()} is not supported");
			}

			if (binding.Lifetime == Lifetime.PerContainer)
			{
				var action = Expression.Lambda(expression).Compile();
				var value = action.DynamicInvoke();
				expression = Expression.Constant(value);
			}
			binding.BindingExpression = expression;
			binding.IsResolved = true;
		}

		private List<Expression> ResolveMethodCallExpression(IBinding binding, IReadOnlyCollection<Expression> parameterExpressions)
		{
			var body = new List<Expression>();
			foreach (var unresolvedParameter in parameterExpressions)
			{
				if (_bindings.TryGetValue(unresolvedParameter.Type, out var dependency))
				{
					if (!dependency.IsResolved)
					{
						ResolveDependency(dependency);
					}
					body.Add(Expression.Assign(unresolvedParameter, dependency.BindingExpression));
				}
				else
				{
					throw new CannotResolveDependencyException($"Error while resolving internal dependencies for {binding.DependencyType}. A instance of type {unresolvedParameter} is needed but was not found in the container.");
				}
			}
			return body;
		}

		public Binding<TDependency> Bind<TDependency>()
		{
			var binding = new Binding<TDependency>();
			_bindings.Add(binding.DependencyType, binding);
			return binding;
		}
	}
}