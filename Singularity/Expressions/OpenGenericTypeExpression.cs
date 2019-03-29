using System;
using System.Linq.Expressions;

namespace Singularity.Expressions
{
    internal class OpenGenericTypeExpression : Expression
    {
        public readonly Type OpenGenericType;

        public OpenGenericTypeExpression(Type openGenericType)
        {
            OpenGenericType = openGenericType;
        }
    }
}
