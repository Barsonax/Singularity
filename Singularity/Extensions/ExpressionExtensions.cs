using System;
using System.Collections.Generic;
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
					return newExpression.Arguments.OfType<ParameterExpression>().ToArray();
				case BlockExpression blockExpression:
					return blockExpression.Variables.ToArray();
				case MethodCallExpression methodCallExpression:
					return methodCallExpression.Arguments.OfType<ParameterExpression>().ToArray();
				default:
					throw new NotSupportedException($"The expression of type {expression.GetType()} is not supported");
			}
		}

		public static IEnumerable<Expression> FlattenExpression(this Expression expression)
		{
			switch (expression)
			{
				case BlockExpression blockExpression:
					return blockExpression.Expressions;
				case Expression ex:
					return new[] { ex };
				default:
					throw new NotSupportedException($"The expression of type {expression.GetType()} is not supported");
			}
		}
	}
}
