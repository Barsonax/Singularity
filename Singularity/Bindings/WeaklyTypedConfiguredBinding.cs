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
        internal Lifetime Lifetime { get; set; }
        internal Action<object>? Finalizer { get; set; }
        private protected readonly WeaklyTypedBinding WeaklyTypedBinding;
        public bool NeedsDispose { get; internal set; }

        internal WeaklyTypedConfiguredBinding(WeaklyTypedBinding weaklyTypedBinding, Expression expression)
        {
            WeaklyTypedBinding = weaklyTypedBinding;
            Expression = expression;
            Lifetime = Lifetime.Transient;
        }

        /// <summary>
        /// Sets the lifetime of the instance(s)
        /// </summary>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public WeaklyTypedConfiguredBinding With(Lifetime lifetime)
        {
            if (!EnumMetadata<Lifetime>.IsValidValue(lifetime)) throw new InvalidLifetimeException(lifetime);
            Lifetime = lifetime;
            return this;
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

    public static class BindingExtensions
    {
        public static T WithDispose<T>(this T binding)
            where T : WeaklyTypedConfiguredBinding
        {
            binding.NeedsDispose = true;
            return binding;
        }
    }
}
