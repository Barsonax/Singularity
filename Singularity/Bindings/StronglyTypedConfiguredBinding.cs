using System;

namespace Singularity.Bindings
{
    /// <summary>
    /// Needed for fluent API. <see cref="StronglyTypedBinding{TDependency}"/> for more info.
    /// </summary>
    public sealed class StronglyTypedConfiguredBinding<TDependency, TInstance>
        where TInstance : class
    {
        private WeaklyTypedConfiguredBinding _weaklyTypedConfiguredBinding;

        internal StronglyTypedConfiguredBinding(WeaklyTypedConfiguredBinding weaklyTypedConfiguredBinding)
        {
            _weaklyTypedConfiguredBinding = weaklyTypedConfiguredBinding;
        }

        /// <summary>
        /// Sets the action that will be executed when the scope ends.
        /// </summary>
        /// <param name="onDeathAction"></param>
        /// <returns></returns>
        public StronglyTypedConfiguredBinding<TDependency, TInstance> WithFinalizer(Action<TInstance> onDeathAction)
        {
            _weaklyTypedConfiguredBinding.Finalizer = obj => onDeathAction((TInstance)obj);
            return this;
        }

        public StronglyTypedConfiguredBinding<TDependency, TInstance> With(DisposeBehavior disposeBehavior)
        {
            _weaklyTypedConfiguredBinding.DisposeBehavior = disposeBehavior;
            return this;
        }

        public StronglyTypedConfiguredBinding<TDependency, TInstance> With(Lifetime lifetime)
        {
            _weaklyTypedConfiguredBinding.Lifetime = lifetime;
            return this;
        }
    }
}