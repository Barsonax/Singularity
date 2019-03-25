using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Singularity.Exceptions;

namespace Singularity.Bindings
{
	public sealed class StronglyTypedDecoratorBinding<TDependency> : IDecoratorBinding
		where TDependency : class
	{
		//public Type DependencyType { get; }
		public Expression? Expression { get; private set; }

		internal StronglyTypedDecoratorBinding()
		{
			Type type = typeof(TDependency);
			if (!type.GetTypeInfo().IsInterface) throw new InterfaceExpectedException($"{type} is not a interface.");
		}

		/// <summary>
		/// Sets the type which should be wrapped by the decorator
		/// </summary>
		/// <typeparam name="TDecorator"></typeparam>
		/// <returns></returns>
		public StronglyTypedDecoratorBinding<TDependency> With<TDecorator>()
			where TDecorator : TDependency
		{
			return With(typeof(TDecorator));
		}

		public StronglyTypedDecoratorBinding<TDependency> With(Type type)
		{
			TypeInfo typeInfo = type.GetTypeInfo();
			if (!typeof(TDependency).GetTypeInfo().IsAssignableFrom(typeInfo)) throw new InterfaceNotImplementedException($"{typeof(TDependency)} is not implemented by {type}");

			Expression = type.AutoResolveConstructorExpression();
			ParameterExpression[] parameters = Expression.GetParameterExpressions();
			if (parameters.All(x => x.Type != typeof(TDependency))) throw new InvalidExpressionArgumentsException($"Cannot decorate {typeof(TDependency)} since the expression to create {type} does not have a parameter for {typeof(TDependency)}");

			return this;
		}
	}
}