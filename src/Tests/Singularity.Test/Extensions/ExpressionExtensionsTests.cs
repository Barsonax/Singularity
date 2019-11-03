using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace Singularity.Test.Extensions
{
	public class ExpressionExtensionsTests
	{
        [Fact]
        public void GetParameterExpressions_Null_Throws()
        {
            Expression? expression = null;
            Assert.Throws<ArgumentNullException>(() => { expression!.GetParameterExpressions(); });
        }

        [Fact]
        public void GetParameterExpressions_InvalidExpressionType_Throws()
        {
            Expression expression = Expression.LeftShift(Expression.Constant(0), Expression.Constant(0));
            Assert.Throws<NotSupportedException>(() => { expression.GetParameterExpressions(); });
        }

        [Fact]
        public void GetParameterExpressions_InvocationExpression_NoError()
        {
            Expression<Func<float, int, object>> lambda = (obj1, obj2) =>  new object();
            InvocationExpression expression = Expression.Invoke(lambda, Expression.Parameter(typeof(float)), Expression.Parameter(typeof(int)));
            ParameterExpression[] parameters = expression.GetParameterExpressions();
            Assert.Equal(2, parameters.Length);
            Assert.Contains(typeof(float), parameters.Select(x => x.Type));
            Assert.Contains(typeof(int), parameters.Select(x => x.Type));
        }

        [Fact]
        public void GetParameterExpressions_UnaryExpression_NoError()
        {
            UnaryExpression expression = Expression.UnaryPlus(Expression.Constant(4));
            ParameterExpression[] parameters = expression.GetParameterExpressions();
            Assert.Empty(parameters);
        }

        [Fact]
        public void GetParameterExpressions_DefaultExpression_NoError()
        {
            DefaultExpression expression = Expression.Default(typeof(Expression));
            ParameterExpression[] parameters = expression.GetParameterExpressions();
            Assert.Empty(parameters);
        }

        [Fact]
		public void GetParameterExpressions_ConstantExpression_NoError()
		{
			ConstantExpression expression = Expression.Constant(5);
			ParameterExpression[] parameters = expression.GetParameterExpressions();
            Assert.Empty(parameters);
        }

		[Fact]
		public void GetParameterExpressions_BlockExpression_NoError()
		{
			BlockExpression expression = Expression.Block(new[] { Expression.Parameter(typeof(int)) }, Expression.Constant(5));
			ParameterExpression[] parameters = expression.GetParameterExpressions();
            Assert.Single(parameters);
        }

		[Fact]
		public void GetParameterExpressions_MethodCallExpression_NoError()
		{
			MethodInfo methodInfo = typeof(ExpressionExtensionsTests).GetRuntimeMethod(nameof(GetParameterExpressions_MethodCallExpression_NoError), new Type[0])!;
			MethodCallExpression expression = Expression.Call(Expression.Constant(this), methodInfo);
			ParameterExpression[] parameters = expression.GetParameterExpressions();
            Assert.Empty(parameters);
        }
	}
}
