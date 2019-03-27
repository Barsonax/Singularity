using System;
using System.Linq.Expressions;

namespace Singularity.Bindings
{
    public sealed class StronglyTypedConfiguredBinding<TDependency, TInstance> : WeaklyTypedConfiguredBinding
        where TInstance : class
    {
        internal StronglyTypedConfiguredBinding(WeaklyTypedBinding weaklyTypedBinding, Expression expression) : base(weaklyTypedBinding, expression)
        {

        }

        public new StronglyTypedConfiguredBinding<TDependency, TInstance> With(CreationMode creationMode)
        {
            CreationMode = creationMode;
            return this;
        }

        public StronglyTypedConfiguredBinding<TDependency, TInstance> OnDeath(Action<TInstance> onDeathAction)
        {
            OnDeathAction = obj => onDeathAction((TInstance)obj);
            return this;
        }
    }
}