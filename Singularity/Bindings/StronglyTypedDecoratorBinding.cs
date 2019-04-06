using System.Linq.Expressions;

namespace Singularity.Bindings
{
    /// <summary>
    /// Represents a strongly typed registration
    /// </summary>
    public sealed class StronglyTypedDecoratorBinding<TDependency> : WeaklyTypedDecoratorBinding
        where TDependency : class
    {
        internal StronglyTypedDecoratorBinding(Expression expression) : base(typeof(TDependency), expression)
        {

        }
    }
}