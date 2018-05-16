using System;
using System.Linq.Expressions;

namespace Singularity.Bindings
{
	public class StronglyTypedConfiguredBinding<TDependency, TInstance> : IConfiguredBinding
		where TInstance : class
	{
		public Expression Expression { get; set; }
		public Lifetime Lifetime { get; private set; }
		public Action<TInstance> OnDeathAction { get; private set; }
		Action<object> IConfiguredBinding.OnDeath => OnDeathAction != null ? (Action<object>)(obj => OnDeathAction((TInstance)obj)) : null;

		public StronglyTypedConfiguredBinding(Expression expression)
		{
			Expression = expression;
		}

		public StronglyTypedConfiguredBinding<TDependency, TInstance> With(Lifetime lifetime)
		{
			Lifetime = lifetime;
			return this;
		}

		public StronglyTypedConfiguredBinding<TDependency, TInstance> OnDeath(Action<TInstance> onDeathAction)
		{
			OnDeathAction = onDeathAction;
			return this;
		}
	}
}