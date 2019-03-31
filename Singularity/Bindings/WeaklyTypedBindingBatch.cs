using System.Collections.ObjectModel;

namespace Singularity.Bindings
{
    /// <summary>
    /// Represents a batch of <see cref="WeaklyTypedConfiguredBinding"/>s
    /// </summary>
    public class WeaklyTypedBindingBatch
    {
        internal ReadOnlyCollection<WeaklyTypedConfiguredBinding> WeaklyTypedBindings { get; }

        internal WeaklyTypedBindingBatch(ReadOnlyCollection<WeaklyTypedConfiguredBinding> weaklyTypedBindings)
        {
            WeaklyTypedBindings = weaklyTypedBindings;
        }

        /// <summary>
        /// Sets the <see cref="CreationMode"/> on all <see cref="WeaklyTypedConfiguredBinding"/>s in this batch.
        /// </summary>
        /// <param name="creationMode"></param>
        /// <returns></returns>
        public WeaklyTypedBindingBatch With(CreationMode creationMode)
        {
            foreach (WeaklyTypedConfiguredBinding weaklyTypedConfiguredBinding in WeaklyTypedBindings)
            {
                weaklyTypedConfiguredBinding.With(creationMode);
            }

            return this;
        }
    }
}
