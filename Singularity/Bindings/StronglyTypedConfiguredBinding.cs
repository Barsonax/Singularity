using System;
using System.Linq.Expressions;

using Singularity.Enums;

namespace Singularity.Bindings
{
	public sealed class StronglyTypedConfiguredBinding<TDependency, TInstance> : IConfiguredBinding
		where TInstance : class
	{
		public Expression Expression { get; }
		public Lifetime Lifetime { get; private set; }
		public Action<TInstance>? OnDeathAction { get; private set; }
		Action<object>? IConfiguredBinding.OnDeath => OnDeathAction != null ? (Action<object>)(obj => OnDeathAction((TInstance)obj)) : null;

		internal StronglyTypedConfiguredBinding(Expression expression)
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