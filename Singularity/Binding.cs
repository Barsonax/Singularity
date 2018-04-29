using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity
{
	public class Binding<TDependency> : IBinding
	{
		public Type DependencyType { get; } = typeof(TDependency);
		public Expression BindingExpression { get; set; }
		public Lifetime Lifetime { get; private set; }
		public bool IsResolved { get; set; }

		public Binding<TDependency> To<TInstance>()
			where TInstance : TDependency
		{
			var type = typeof(TInstance);
			var constructor = type.GetTypeInfo().DeclaredConstructors.First();
			var parameters = constructor.GetParameters();
			var parameterExpressions = new ParameterExpression[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				parameterExpressions[i] = Expression.Parameter(parameters[i].ParameterType);
			}
			BindingExpression = Expression.New(constructor, parameterExpressions);
			return this;
		}

		public Binding<TDependency> To<TInstance>(Expression<Func<TInstance>> expression)
			where TInstance : TDependency
		{
			BindingExpression = expression;
			return this;
		}

		public Binding<TDependency> To<TInstance, TP1>(Expression<Func<TP1, TInstance>> expression)
			where TInstance : TDependency
		{
			BindingExpression = expression;
			return this;
		}

		public Binding<TDependency> SetLifetime(Lifetime lifetime)
		{
			Lifetime = lifetime;
			return this;
		}
	}
}
