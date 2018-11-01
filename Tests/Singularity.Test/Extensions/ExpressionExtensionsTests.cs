using System;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace Singularity.Test.Extensions
{
	public class ExpressionExtensionsTests
	{
		[Fact]
		public void GetParameterExpressions_InvalidExpressionType_Throws()
		{
			DefaultExpression expression = Expression.Default(typeof(Expression));
			Assert.Throws<NotSupportedException>(() => { expression.GetParameterExpressions(); });
		}

		[Fact]
		public void GetParameterExpressions_ConstantExpression_NoError()
		{
			ConstantExpression expression = Expression.Constant(5);
			ParameterExpression[] parameters = expression.GetParameterExpressions();
			Assert.Equal(0, parameters.Length);
		}

		[Fact]
		public void GetParameterExpressions_BlockExpression_NoError()
		{
			BlockExpression expression = Expression.Block(new[] { Expression.Parameter(typeof(int)) }, Expression.Constant(5));
			ParameterExpression[] parameters = expression.GetParameterExpressions();
			Assert.Equal(1, parameters.Length);
		}

		[Fact]
		public void GetParameterExpressions_MethodCallExpression_NoError()
		{
			MethodInfo methodInfo = typeof(ExpressionExtensionsTests).GetRuntimeMethod(nameof(GetParameterExpressions_MethodCallExpression_NoError), new Type[0]);
			MethodCallExpression expression = Expression.Call(Expression.Constant(this), methodInfo);
			ParameterExpression[] parameters = expression.GetParameterExpressions();
			Assert.Equal(0, parameters.Length);
		}
	}
}
