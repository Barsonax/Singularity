using System;
using System.Linq.Expressions;

namespace Singularity.Bindings
{
	public class WeaklyTypedConfiguredBinding : IConfiguredBinding
	{
		public Expression Expression { get; }
		public Lifetime Lifetime { get; }
		public Action<object> OnDeath { get; }

		public WeaklyTypedConfiguredBinding(Expression expression, Lifetime lifetime, Action<object> onDeath)
		{
			Expression = expression;
			Lifetime = lifetime;
			OnDeath = onDeath;
		}
	}
}