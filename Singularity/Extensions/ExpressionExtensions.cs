using System;
using System.Linq;
using System.Linq.Expressions;

namespace Singularity.Extensions
{
	public static class ExpressionExtensions
    {
        public static ParameterExpression[] GetParameterExpressions(this Expression expression)
        {
            switch (expression)
            {
                case ConstantExpression _:
                    return new ParameterExpression[0];
                case LambdaExpression lambdaExpression:
                    return lambdaExpression.Parameters.ToArray();
                case NewExpression newExpression:
                    return newExpression.Arguments.Select(x => (ParameterExpression)x).ToArray();
                case BlockExpression blockExpression:
                    return blockExpression.Variables.ToArray();
                default:
                    throw new NotSupportedException($"The expression of type {expression.GetType()} is not supported");
            }
        }
    }
}
