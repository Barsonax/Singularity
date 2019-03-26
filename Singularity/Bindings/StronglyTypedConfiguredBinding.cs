using System;
using System.Linq.Expressions;

namespace Singularity.Bindings
{
    public sealed class StronglyTypedConfiguredBinding<TDependency, TInstance> : WeaklyTypedConfiguredBinding
        where TInstance : class
    {
        internal StronglyTypedConfiguredBinding(Expression expression) : base(expression)
        {

        }

        public new StronglyTypedConfiguredBinding<TDependency, TInstance> With(ILifetime lifetime)
        {
            Lifetime = lifetime;
            return this;
        }

        public StronglyTypedConfiguredBinding<TDependency, TInstance> OnDeath(Action<TInstance> onDeathAction)
        {
            OnDeathAction = obj => onDeathAction((TInstance)obj);
            return this;
        }

        public override WeaklyTypedConfiguredBinding OnDeath(Action<object> onDeathAction)
        {
            OnDeathAction = obj => onDeathAction((TInstance)obj);
            return this;
        }
    }
}