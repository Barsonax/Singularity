using System;
using System.Linq.Expressions;

namespace Singularity.Bindings
{
    /// <summary>
    /// Needed for fluent API. <see cref="WeaklyTypedBinding"/> for more info.
    /// </summary>
    public class WeaklyTypedConfiguredBinding
    {
        internal Expression Expression { get; }
        public Lifetime Lifetime { get; internal set; }
        internal Action<object>? Finalizer { get; set; }
        private protected readonly WeaklyTypedBinding WeaklyTypedBinding;
        public Dispose NeedsDispose { get; internal set; }

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
    }
}
