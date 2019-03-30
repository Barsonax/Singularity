using System;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Exceptions;

namespace Singularity.Bindings
{
    public abstract class WeaklyTypedDecoratorBinding
    {
        public Type DependencyType { get; }
        public Expression? Expression { get; internal set; }

        internal WeaklyTypedDecoratorBinding(Type dependencyType)
        {
            if (!dependencyType.GetTypeInfo().IsInterface) throw new InterfaceExpectedException($"{dependencyType} is not a interface.");
            DependencyType = dependencyType;
        }
    }
}
