using System;
using System.Linq.Expressions;

namespace Singularity.Bindings
{
    public abstract class WeaklyTypedConfiguredBinding
    {
        public Expression Expression { get; }
        public CreationMode CreationMode { get; protected set; }
        public Action<object>? OnDeathAction { get; protected set; }
        protected readonly WeaklyTypedBinding _weaklyTypedBinding;

        internal WeaklyTypedConfiguredBinding(WeaklyTypedBinding weaklyTypedBinding, Expression expression)
        {
            _weaklyTypedBinding = weaklyTypedBinding;
            Expression = expression;
            CreationMode = CreationMode.Transient;
        }

        public WeaklyTypedConfiguredBinding With(CreationMode creationMode)
        {
            CreationMode = creationMode;
            return this;
        }

        public WeaklyTypedConfiguredBinding OnDeath(Action<object> onDeathAction)
        {
            OnDeathAction = onDeathAction;
            return this;
        }
    }
}
