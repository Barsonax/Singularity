using System;
using System.Linq.Expressions;

namespace Singularity.Bindings
{
    /// <summary>
    /// Represents a strongly typed registration
    /// </summary>
    public sealed class StronglyTypedDecoratorBinding<TDependency>
        where TDependency : class
    {
        private WeaklyTypedDecoratorBinding _weaklyTypedDecoratorBinding { get; }

        /// <summary>
        /// The dependency type this decorator is used for.
        /// </summary>
        public Type DependencyType => _weaklyTypedDecoratorBinding.DependencyType;

        /// <summary>
        /// A expression to create the decorator.
        /// </summary>
        public Expression Expression => _weaklyTypedDecoratorBinding.Expression;

        internal StronglyTypedDecoratorBinding(WeaklyTypedDecoratorBinding weaklyTypedDecoratorBinding)
        {
            _weaklyTypedDecoratorBinding = weaklyTypedDecoratorBinding;
        }
    }
}