using System;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Exceptions;

namespace Singularity.Bindings
{
    public abstract class WeaklyTypedDecoratorBinding
    {
        public Expression? Expression { get; protected set; }

        internal WeaklyTypedDecoratorBinding(Type dependencyType)
        {
            if (!dependencyType.GetTypeInfo().IsInterface) throw new InterfaceExpectedException($"{dependencyType} is not a interface.");
        }

        public abstract WeaklyTypedDecoratorBinding With(Type decoratorType);
    }
}
