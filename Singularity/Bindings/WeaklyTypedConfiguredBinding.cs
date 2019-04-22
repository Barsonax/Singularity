using Singularity.Exceptions;
using System;
using System.Linq.Expressions;

namespace Singularity.Bindings
{
    /// <summary>
    /// Needed for fluent API. <see cref="WeaklyTypedBinding"/> for more info.
    /// </summary>
    public sealed class WeaklyTypedConfiguredBinding
    {
        internal Expression Expression { get; }

        private Lifetime lifetime;
        public Lifetime Lifetime
        {
            get => lifetime;
            internal set
            {
                if (!EnumMetadata<Lifetime>.IsValidValue(value)) throw new InvalidEnumValue<Lifetime>(value);
                lifetime = value;
            }
        }

        private DisposeBehavior disposeBehavior;
        public DisposeBehavior DisposeBehavior
        {
            get => disposeBehavior;
            internal set
            {
                if (!EnumMetadata<DisposeBehavior>.IsValidValue(value)) throw new InvalidEnumValue<DisposeBehavior>(value);
                disposeBehavior = value;
            }
        }
        internal Action<object>? Finalizer { get; set; }
        private protected readonly WeaklyTypedBinding WeaklyTypedBinding;

        internal WeaklyTypedConfiguredBinding(WeaklyTypedBinding weaklyTypedBinding, Expression expression)
        {
            WeaklyTypedBinding = weaklyTypedBinding;
            Expression = expression;
        }

        /// <summary>
        /// Sets the action that will be executed when the scope ends.
        /// </summary>
        /// <param name="onDeathAction"></param>
        /// <returns></returns>
        public WeaklyTypedConfiguredBinding WithFinalizer(Action<object> onDeathAction)
        {
            Finalizer = onDeathAction;
            return this;
        }

        public WeaklyTypedConfiguredBinding With(DisposeBehavior disposeBehavior)
        {
            DisposeBehavior = disposeBehavior;
            return this;
        }

        public WeaklyTypedConfiguredBinding With(Lifetime lifetime)
        {
            Lifetime = lifetime;
            return this;
        }
    }
}
