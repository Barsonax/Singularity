using Singularity.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity
{
	public class StronglyTypedDecoratorBinding<TDependency> : IDecoratorBinding
	{
		public Type DependencyType { get; }
		public Expression Expression { get; private set; }

		public StronglyTypedDecoratorBinding()
		{
			var type = typeof(TDependency);
			if (!type.GetTypeInfo().IsInterface) throw new InterfaceExpectedException($"{type} is not a interface.");
			DependencyType = type;			
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
			var typeInfo = type.GetTypeInfo();
			if (!DependencyType.GetTypeInfo().IsAssignableFrom(typeInfo)) throw new InterfaceNotImplementedException($"{DependencyType} is not implemented by {type}");

			Expression = type.AutoResolveConstructor();
			var parameters = Expression.GetParameterExpressions();
			if (parameters.All(x => x.Type != DependencyType)) throw new InvalidExpressionArgumentsException($"Cannot decorate {DependencyType} since the expression to create {type} does not have a parameter for {DependencyType}");

			return this;
		}
	}
}