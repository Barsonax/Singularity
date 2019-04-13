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
        /// Sets the <see cref="Lifetime"/> on all <see cref="WeaklyTypedConfiguredBinding"/>s in this batch.
        /// </summary>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public WeaklyTypedBindingBatch With(Lifetime lifetime)
        {
            foreach (WeaklyTypedConfiguredBinding weaklyTypedConfiguredBinding in WeaklyTypedBindings)
            {
                weaklyTypedConfiguredBinding.With(lifetime);
            }

            return this;
        }
    }
}
