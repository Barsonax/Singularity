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
			var expression = Expression.Default(typeof(Expression));
			Assert.Throws<NotSupportedException>(() => { expression.GetParameterExpressions(); });
		}

		[Fact]
		public void GetParameterExpressions_ConstantExpression_NoError()
		{
			var expression = Expression.Constant(5);
			var parameters = expression.GetParameterExpressions();
			Assert.Equal(0, parameters.Length);
		}

		[Fact]
		public void GetParameterExpressions_BlockExpression_NoError()
		{
			var expression = Expression.Block(new[] { Expression.Parameter(typeof(int)) }, Expression.Constant(5));
			var parameters = expression.GetParameterExpressions();
			Assert.Equal(1, parameters.Length);
		}

		[Fact]
		public void GetParameterExpressions_MethodCallExpression_NoError()
		{
			var methodInfo = typeof(ExpressionExtensionsTests).GetRuntimeMethod(nameof(GetParameterExpressions_MethodCallExpression_NoError), new Type[0]);
			var expression = Expression.Call(Expression.Constant(this), methodInfo);
			var parameters = expression.GetParameterExpressions();
			Assert.Equal(0, parameters.Length);
		}
	}
}
