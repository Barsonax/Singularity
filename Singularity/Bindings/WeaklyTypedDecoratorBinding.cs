using System;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Exceptions;

namespace Singularity.Bindings
{
    /// <summary>
    /// Represents a weakly typed decorator registration
    /// </summary>
    public class WeaklyTypedDecoratorBinding
    {
        /// <summary>
        /// The dependency type this decorator is used for.
        /// </summary>
        public Type DependencyType { get; }

        /// <summary>
        /// A expression to create the decorator.
        /// </summary>
        public Expression? Expression { get; internal set; }

        internal WeaklyTypedDecoratorBinding(Type dependencyType)
        {
            if (!dependencyType.GetTypeInfo().IsInterface) throw new InterfaceExpectedException($"{dependencyType} is not a interface.");
            DependencyType = dependencyType;
        }
    }
}
