using System;
using System.Linq.Expressions;
using Singularity.Exceptions;

namespace Singularity.Bindings
{
    /// <summary>
    /// Needed for fluent API. <see cref="WeaklyTypedBinding"/> for more info.
    /// </summary>
    public class WeaklyTypedConfiguredBinding
    {
        internal Expression Expression { get; }
        internal CreationMode CreationMode { get; set; }
        internal Action<object>? OnDeathAction { get; set; }
        private protected readonly WeaklyTypedBinding WeaklyTypedBinding;

        internal WeaklyTypedConfiguredBinding(WeaklyTypedBinding weaklyTypedBinding, Expression expression)
        {
            WeaklyTypedBinding = weaklyTypedBinding;
            Expression = expression;
            CreationMode = CreationMode.Transient;
        }

        /// <summary>
        /// Sets the lifetime of the instance(s)
        /// </summary>
        /// <param name="creationMode"></param>
        /// <returns></returns>
        public WeaklyTypedConfiguredBinding With(CreationMode creationMode)
        {
            if (!EnumMetadata<CreationMode>.IsValidValue(creationMode)) throw new InvalidLifetimeException(creationMode);
            CreationMode = creationMode;
            return this;
        }

        /// <summary>
        /// Sets the action that will be executed when the scope ends.
        /// </summary>
        /// <param name="onDeathAction"></param>
        /// <returns></returns>
        public WeaklyTypedConfiguredBinding OnDeath(Action<object> onDeathAction)
        {
            OnDeathAction = onDeathAction;
            return this;
        }
    }
}
