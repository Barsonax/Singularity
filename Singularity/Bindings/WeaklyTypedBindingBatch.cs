using System.Collections.ObjectModel;

namespace Singularity.Bindings
{
    public class WeaklyTypedBindingBatch
    {
        public ReadOnlyCollection<WeaklyTypedConfiguredBinding> WeaklyTypedBindings { get; }

        public WeaklyTypedBindingBatch(ReadOnlyCollection<WeaklyTypedConfiguredBinding> weaklyTypedBindings)
        {
            WeaklyTypedBindings = weaklyTypedBindings;
        }

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
