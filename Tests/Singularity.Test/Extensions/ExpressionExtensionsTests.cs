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
            Expression expression = Expression.LeftShift(Expression.Constant(0), Expression.Constant(0));
            Assert.Throws<NotSupportedException>(() => { expression.GetParameterExpressions(); });
        }

        [Fact]
        public void GetParameterExpressions_UnaryExpression_NoError()
        {
            UnaryExpression expression = Expression.UnaryPlus(Expression.Constant(4));
            ParameterExpression[] parameters = expression.GetParameterExpressions();
            Assert.Equal(0, parameters.Length);
        }

        [Fact]
        public void GetParameterExpressions_DefaultExpression_NoError()
        {
            DefaultExpression expression = Expression.Default(typeof(Expression));
            ParameterExpression[] parameters = expression.GetParameterExpressions();
            Assert.Equal(0, parameters.Length);
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
