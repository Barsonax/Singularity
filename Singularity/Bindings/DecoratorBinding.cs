using Singularity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Singularity
{
	public class DecoratorBinding<TDecorator> : IDecoratorBinding
	{
		public Type DependencyType { get; private set; }
		public Expression Expression { get; set; }
		private BindingConfig _bindingConfig;

		public DecoratorBinding(BindingConfig bindingConfig)
		{
			_bindingConfig = bindingConfig;
			Expression = typeof(TDecorator).AutoResolveConstructor();
		}

		public DecoratorBinding<TDecorator> To(Type type)
		{
			DependencyType = type;

			var parameters = Expression.GetParameterExpressions();
			if (parameters.All(x => x.Type != type)) throw new InvalidExpressionArgumentsException($"The expression for {Expression.Type} does not have a parameter for {type}");
			if (!_bindingConfig.Decorators.TryGetValue(type, out var decorators))
			{
				decorators = new List<IDecoratorBinding>();
				_bindingConfig.Decorators.Add(type, decorators);
			}
			decorators.Add(this);
			return this;
		}

		public DecoratorBinding<TDecorator> To<TDependency>()
		{
			return To(typeof(TDependency));
		}
	}
}