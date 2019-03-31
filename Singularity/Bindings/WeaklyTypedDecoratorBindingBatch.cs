using System.Collections.ObjectModel;

namespace Singularity.Bindings
{
    /// <summary>
    /// Represents a batch of <see cref="WeaklyTypedDecoratorBinding"/>s
    /// </summary>
    public class WeaklyTypedDecoratorBindingBatch
    {
        internal ReadOnlyCollection<WeaklyTypedDecoratorBinding> WeaklyTypedDecoratorBindings { get; }

        internal WeaklyTypedDecoratorBindingBatch(ReadOnlyCollection<WeaklyTypedDecoratorBinding> weaklyTypedDecoratorBindings)
        {
            WeaklyTypedDecoratorBindings = weaklyTypedDecoratorBindings;
        }
    }
}
