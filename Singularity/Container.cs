using Singularity.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity
{
	public class Container
	{
		//TODO handle disposable objects.

		private readonly DependencyGraph _dependencyGraph;

		private readonly Dictionary<Type, Action<object>> _injectionCache = new Dictionary<Type, Action<object>>(ReferenceEqualityComparer<Type>.Instance);
		private readonly Dictionary<Type, Func<object>> _getInstanceCache = new Dictionary<Type, Func<object>>(ReferenceEqualityComparer<Type>.Instance);


		public Container(BindingConfig bindingConfig)
		{
			bindingConfig.ValidateBindings();
			_dependencyGraph = new DependencyGraph(bindingConfig);
		}

		public void Inject(IEnumerable<object> instances)
		{
			foreach (var instance in instances)
			{
				Inject(instance);
			}
		}

		public void Inject(object instance)
		{
			var type = instance.GetType();
			if (!_injectionCache.TryGetValue(type, out var action))
			{
				action = GenerateInjectionExpression(type);
				_injectionCache.Add(type, action);
			}
			action.Invoke(instance);
		}

		public T GetInstance<T>() where T : class
		{
			var value = GetInstance(typeof(T));
			return (T)value;
		}

		public object GetInstance(Type type)
		{
			if (!_getInstanceCache.TryGetValue(type, out var action))
			{
				action = GenerateGetInstanceExpression(type);
				_getInstanceCache.Add(type, action);
			}
			var value = action.Invoke();
			return value;
		}

		private Func<object> GenerateGetInstanceExpression(Type type)
		{
			if (_dependencyGraph.Dependencies.TryGetValue(type, out var dependencyNode))
			{
				if (dependencyNode.Expression is ConstantExpression constantExpression)
				{
					return () => constantExpression.Value;
				}
				return Expression.Lambda<Func<object>>(dependencyNode.Expression).Compile();
			}
			else
			{
				throw new DependencyNotFoundException($"No configured dependency found for {type}");
			}
		}

		private Action<object> GenerateInjectionExpression(Type type)
		{
			var instanceParameter = Expression.Parameter(typeof(object));

			var body = new List<Expression>();
			var instanceCasted = Expression.Variable(type, "instanceCasted");
			body.Add(instanceCasted);
			body.Add(Expression.Assign(instanceCasted, Expression.Convert(instanceParameter, type)));
			foreach (var methodInfo in type.GetRuntimeMethods())
			{
				if (methodInfo.CustomAttributes.All(x => x.AttributeType != typeof(InjectAttribute))) continue;
				var parameterTypes = methodInfo.GetParameters();
				var parameterExpressions = new Expression[parameterTypes.Length];
				for (int i = 0; i < parameterTypes.Length; i++)
				{
					var parameterType = parameterTypes[i].ParameterType;
					if (_dependencyGraph.Dependencies.TryGetValue(parameterType, out var dependencyNode))
					{
						parameterExpressions[i] = dependencyNode.Expression;
					}
					else
					{
						throw new DependencyNotFoundException($"No configured dependency found for {parameterType}");
					}
				}

				body.Add(Expression.Call(instanceCasted, methodInfo, parameterExpressions));
			}
			var block = Expression.Block(new[] { instanceCasted }, body);
			var expressionTree = Expression.Lambda<Action<object>>(block, instanceParameter);

			var action = expressionTree.Compile();
			return action;
		}
	}
}