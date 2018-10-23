using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Singularity.Bindings;
using Singularity.Exceptions;
using Singularity.Extensions;

namespace Singularity.Graph
{
	public class DependencyGraph
	{
		public ReadOnlyDictionary<Type, Dependency> Dependencies { get; }

		public DependencyGraph(IEnumerable<IBinding> bindings, IEnumerable<IDependencyExpressionGenerator> dependencyExpressionGenerators, IReadOnlyDictionary<Type, Dependency> parentDependencies = null)
		{
			var unresolvedDependencies = MergeBindings(bindings, parentDependencies);

			var graph = new Graph<UnresolvedDependency>(unresolvedDependencies.Values);
			var updateOrder = graph.GetUpdateOrder(x => GetDependencies(x, unresolvedDependencies));

			var dependencies = new Dictionary<Type, Dependency>();
			foreach (var group in updateOrder)
			{
				foreach (var unresolvedDependency in group)
				{
					var resolvedDependency = ResolveDependency(unresolvedDependency.DependencyType, unresolvedDependency, dependencyExpressionGenerators, dependencies);
					dependencies.Add(unresolvedDependency.DependencyType, new Dependency(unresolvedDependency, resolvedDependency));
				}
			} 
			Dependencies = new ReadOnlyDictionary<Type, Dependency>(dependencies);
		}

		private static Dictionary<Type, UnresolvedDependency> MergeBindings(IEnumerable<IBinding> childBindingConfig, IReadOnlyDictionary<Type, Dependency> parentDependencyGraph)
		{
			var unresolvedChildDependencies = new Dictionary<Type, UnresolvedDependency>();
			foreach (var binding in childBindingConfig)
			{
				unresolvedChildDependencies.Add(binding.DependencyType, new UnresolvedDependency(binding.DependencyType, binding.Expression, binding.Lifetime, binding.Decorators, binding.OnDeath));
			}

			if (parentDependencyGraph != null)
			{
				foreach (var parentDependency in parentDependencyGraph.Values)
				{
					if (unresolvedChildDependencies.TryGetValue(parentDependency.UnresolvedDependency.DependencyType, out var unresolvedChildDependency))
					{
						if (unresolvedChildDependency.Expression != null) continue;

						List<IDecoratorBinding> decorators;
						Expression expression;
						Action<object> onDeathAction;
						if (parentDependency.UnresolvedDependency.Lifetime == Lifetime.PerContainer)
						{
							expression = parentDependencyGraph[unresolvedChildDependency.DependencyType].ResolvedDependency.Expression;
							decorators = unresolvedChildDependency.Decorators.ToList();
							onDeathAction = null;
						}
						else
						{
							expression = parentDependency.UnresolvedDependency.Expression;
							decorators = parentDependency.UnresolvedDependency.Decorators.Concat(unresolvedChildDependency.Decorators).ToList();
							onDeathAction = parentDependency.UnresolvedDependency.OnDeath;
						}

						var readonlyBinding = new UnresolvedDependency(unresolvedChildDependency.DependencyType, expression, parentDependency.UnresolvedDependency.Lifetime, decorators, onDeathAction);
						unresolvedChildDependencies[unresolvedChildDependency.DependencyType] = readonlyBinding;
					}
					else
					{
						if (parentDependency.UnresolvedDependency.Lifetime == Lifetime.PerContainer)
						{

							var readonlyBinding = new UnresolvedDependency(parentDependency.UnresolvedDependency.DependencyType,
								parentDependency.UnresolvedDependency.Expression, parentDependency.UnresolvedDependency.Lifetime,
								parentDependency.UnresolvedDependency.Decorators, null);
							unresolvedChildDependencies.Add(parentDependency.UnresolvedDependency.DependencyType, readonlyBinding);
						}
						else
						{
							unresolvedChildDependencies.Add(parentDependency.UnresolvedDependency.DependencyType, parentDependency.UnresolvedDependency);
						}
					}
				}
			}

			return unresolvedChildDependencies;
		}

		private IEnumerable<UnresolvedDependency> GetDependencies(UnresolvedDependency unresolvedDependency, Dictionary<Type, UnresolvedDependency> unresolvedDependencies)
		{
			var resolvedDependencies = new List<UnresolvedDependency>();
			if (unresolvedDependency.Expression.GetParameterExpressions()
				.TryExecute(dependencyType => { resolvedDependencies.Add(GetDependency(dependencyType.Type, unresolvedDependencies)); }, out var dependencyExceptions))
			{
				throw new SingularityAggregateException($"Could not find all dependencies for {unresolvedDependency.Expression.Type}", dependencyExceptions);
			}

			if (unresolvedDependency.Decorators
				.SelectMany(x => x.Expression.GetParameterExpressions())
				.Where(x => x.Type != unresolvedDependency.DependencyType)
				.TryExecute(parameterExpression => { resolvedDependencies.Add(GetDependency(parameterExpression.Type, unresolvedDependencies)); }, out var decoratorExceptions))
			{
				throw new SingularityAggregateException($"Could not find all decorator dependencies for {unresolvedDependency.Expression.Type}", decoratorExceptions);
			}

			return resolvedDependencies;
		}

		private TValue GetDependency<TValue>(Type type, Dictionary<Type, TValue> unresolvedDependencies)
		{
			if (unresolvedDependencies.TryGetValue(type, out var parent)) return parent;
			throw new DependencyNotFoundException(type);
		}

		private ResolvedDependency ResolveDependency(Type dependencyType, UnresolvedDependency unresolvedDependency, IEnumerable<IDependencyExpressionGenerator> dependencyExpressionGenerators, Dictionary<Type, Dependency> resolvedDependencies)
		{
			var expression = GenerateDependencyExpression(dependencyType, unresolvedDependency, dependencyExpressionGenerators, resolvedDependencies);

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
					throw new ArgumentOutOfRangeException(nameof(unresolvedDependency.Lifetime));
			}
			return new ResolvedDependency(expression);
		}

		private Expression GenerateDependencyExpression(Type dependencyType, UnresolvedDependency binding, IEnumerable<IDependencyExpressionGenerator> dependencyExpressionGenerators, Dictionary<Type, Dependency> resolvedDependencies)
		{
			var expression = binding.Expression is LambdaExpression lambdaExpression ? lambdaExpression.Body : binding.Expression;
			var body = new List<Expression>();
			var parameters = new List<ParameterExpression>();
			parameters.AddRange(expression.GetParameterExpressions());

			var instanceParameter = Expression.Variable(dependencyType, $"{expression.Type} instance");
			body.Add(Expression.Assign(instanceParameter, Expression.Convert(expression, dependencyType)));
			foreach (var dependencyExpressionGenerator in dependencyExpressionGenerators)
			{
				dependencyExpressionGenerator.Generate(binding, instanceParameter, parameters, body);
			}

			foreach (var unresolvedParameter in parameters)
			{
				var dependency = GetDependency(unresolvedParameter.Type, resolvedDependencies);
				body.Insert(0, Expression.Assign(unresolvedParameter, dependency.ResolvedDependency.Expression));
			}

			if (body.Last().Type == typeof(void)) body.Add(instanceParameter);
			return body.Count == 1 ? expression : Expression.Block(parameters.Concat(new[] { instanceParameter }), body);
		}
	}
}