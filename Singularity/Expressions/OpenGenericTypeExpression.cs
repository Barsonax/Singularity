using System;
using System.Linq.Expressions;

namespace Singularity.Expressions
{
    internal class OpenGenericTypeExpression : Expression
    {
        public OpenGenericTypeExpression(Type openGenericType)
        {
            Type = openGenericType;
        }

        public override Type Type { get; }
    }
}
