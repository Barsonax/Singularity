using Singularity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity
{
	public class DecoratorBinding<TDecorator> : IDecoratorBinding
	{
		public Type DependencyType { get; private set; }
		public Expression Expression { get; set; }
		private readonly BindingConfig _bindingConfig;

		public DecoratorBinding(BindingConfig bindingConfig)
		{
			_bindingConfig = bindingConfig;
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
            DependencyType = type;

			var parameters = Expression.GetParameterExpressions();
			if (parameters.All(x => x.Type != type)) throw new InvalidExpressionArgumentsException($"Cannot decorate {type} since the expression to create {typeof(TDecorator)} does not have a parameter for {type}");
			if (!_bindingConfig.Decorators.TryGetValue(type, out var decorators))
			{
				decorators = new List<IDecoratorBinding>();
				_bindingConfig.Decorators.Add(type, decorators);
			}
			decorators.Add(this);
			return this;
		}
	}
}