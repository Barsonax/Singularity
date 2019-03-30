namespace Singularity.Bindings
{
    public sealed class StronglyTypedDecoratorBinding<TDependency> : WeaklyTypedDecoratorBinding
        where TDependency : class
    {
        internal StronglyTypedDecoratorBinding() : base(typeof(TDependency))
        {

        }
    }
}