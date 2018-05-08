using Singularity.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity
{
	public class DecoratorBinding<TDecorator> : IDecoratorBinding
	{
		public Type DependencyType { get; private set; }
		public Expression Expression { get; set; }

		public DecoratorBinding()
		{
			Expression = typeof(TDecorator).AutoResolveConstructor();
		}

        /// <summary>
        /// Sets the type which should be wrapped by the decorator
        /// </summary>
        /// <typeparam name="TDependency"></typeparam>
        /// <returns></returns>
	    public DecoratorBinding<TDecorator> On<TDependency>()
	    {
	        return On(typeof(TDependency));
	    }

        public DecoratorBinding<TDecorator> On(Type type)
		{
		    var typeInfo = type.GetTypeInfo();
		    if (!typeInfo.IsInterface) throw new InterfaceExpectedException($"{type} is not a interface.");
		    if (!typeInfo.IsAssignableFrom(typeof(TDecorator).GetTypeInfo())) throw new InterfaceNotImplementedException($"{type} is not implemented by {typeof(TDecorator)}");          

			var parameters = Expression.GetParameterExpressions();
			if (parameters.All(x => x.Type != type)) throw new InvalidExpressionArgumentsException($"Cannot decorate {type} since the expression to create {typeof(TDecorator)} does not have a parameter for {type}");

			DependencyType = type;
			return this;
		}
	}
}