using System;
using System.Linq.Expressions;
using Singularity.Exceptions;

namespace Singularity.Bindings
{
    /// <summary>
    /// Needed for fluent API. <see cref="StronglyTypedBinding{TDependency}"/> for more info.
    /// </summary>
    public sealed class StronglyTypedConfiguredBinding<TDependency, TInstance> : WeaklyTypedConfiguredBinding
        where TInstance : class
    {
        internal StronglyTypedConfiguredBinding(WeaklyTypedBinding weaklyTypedBinding, Expression expression) : base(weaklyTypedBinding, expression)
        {

        }

        /// <summary>
        /// Sets the lifetime of the instance(s)
        /// </summary>
        /// <param name="creationMode"></param>
        /// <returns></returns>
        public new StronglyTypedConfiguredBinding<TDependency, TInstance> With(CreationMode creationMode)
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
        public StronglyTypedConfiguredBinding<TDependency, TInstance> OnDeath(Action<TInstance> onDeathAction)
        {
            OnDeathAction = obj => onDeathAction((TInstance)obj);
            return this;
        }
    }
}