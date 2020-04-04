using Singularity.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Singularity
{
	internal static class ExpressionExtensions
	{
        public static Type GetReturnType(this Expression expression)
        {
            switch (expression)
            {
                case LambdaExpression lambdaExpression:
                    return lambdaExpression.ReturnType;
                default:
                    return expression.Type;
            }
        }

        public static IEnumerable<ParameterExpression> GetParameterExpressions(this IEnumerable<Expression> expressions)
        {
            return expressions.SelectMany(x => x.GetParameterExpressions());
        }

        public static ParameterExpression[] GetParameterExpressions(this Expression expression)
		{
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            switch (expression)
			{
				case ConstantExpression _:
                case DefaultExpression _:
                case UnaryExpression _:
                case MemberExpression _:
                    return new ParameterExpression[0];
                case LambdaExpression lambdaExpression:
					return lambdaExpression.Parameters.ToArray();
				case NewExpression newExpression:
					return newExpression.Arguments.OfType<ParameterExpression>().ToArray();
				case BlockExpression blockExpression:
                    return blockExpression.Variables.Where(x => x.NodeType != ExpressionType.RuntimeVariables).ToArray();
				case MethodCallExpression methodCallExpression:
					return methodCallExpression.Arguments.OfType<ParameterExpression>().ToArray();
                case InvocationExpression invocationExpression:
                    return invocationExpression.Arguments.OfType<ParameterExpression>().ToArray();
                default:
					throw new NotSupportedException($"The expression of type {expression.GetType()} is not supported");
			}
		}
	}
}
