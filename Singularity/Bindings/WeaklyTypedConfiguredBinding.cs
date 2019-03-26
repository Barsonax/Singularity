using System;
using System.Linq.Expressions;

namespace Singularity.Bindings
{
    public abstract class WeaklyTypedConfiguredBinding
    {
        public Expression Expression { get; }
        public ILifetime Lifetime { get; protected set; }
        public Action<object>? OnDeathAction { get; protected set; }

        internal WeaklyTypedConfiguredBinding(Expression expression)
        {
            Expression = expression;
            Lifetime = Lifetimes.Transient;
        }

        public WeaklyTypedConfiguredBinding With(ILifetime lifetime)
        {
            Lifetime = lifetime;
            return this;
        }

        public abstract WeaklyTypedConfiguredBinding OnDeath(Action<object> onDeathAction);
    }
}
