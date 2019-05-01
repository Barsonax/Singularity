using System;
using System.Linq.Expressions;

namespace Singularity.Expressions
{
    /// <summary>
    /// A expression that is used to dynamically generate new bindings.
    /// Do not compile and invoke the delegate directly.
    /// </summary>
    internal sealed class AbstractBindingExpression : Expression
    {
        public AbstractBindingExpression(Type instanceType)
        {
            Type = instanceType;
        }

        public override Type Type { get; }
    }
}
