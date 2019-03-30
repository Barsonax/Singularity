using System.Collections.ObjectModel;

namespace Singularity.Bindings
{
    public class WeaklyTypedDecoratorBindingBatch
    {
        public ReadOnlyCollection<WeaklyTypedDecoratorBinding> WeaklyTypedDecoratorBindings { get; }

        public WeaklyTypedDecoratorBindingBatch(ReadOnlyCollection<WeaklyTypedDecoratorBinding> weaklyTypedDecoratorBindings)
        {
            WeaklyTypedDecoratorBindings = weaklyTypedDecoratorBindings;
        }
    }
}
