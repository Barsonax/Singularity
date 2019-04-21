using System;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Exceptions;

namespace Singularity.Bindings
{
    /// <summary>
    /// Represents a weakly typed decorator registration
    /// </summary>
    public sealed class WeaklyTypedDecoratorBinding
    {
        /// <summary>
        /// The dependency type this decorator is used for.
        /// </summary>
        public Type DependencyType { get; }

        /// <summary>
        /// A expression to create the decorator.
        /// </summary>
        public Expression Expression { get; }

        internal WeaklyTypedDecoratorBinding(Type dependencyType, Expression expression)
        {
            DependencyType = dependencyType ?? throw new ArgumentNullException(nameof(dependencyType));
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            if (!dependencyType.GetTypeInfo().IsInterface) throw new InterfaceExpectedException($"{dependencyType} is not a interface.");
        }
    }
}
