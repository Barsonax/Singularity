using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Expressions;
using Singularity.FastExpressionCompiler;

namespace Singularity
{
    /// <summary>
    /// The same instance will be returned as long as it is requested in the same <see cref="Scoped"/>.
    /// </summary>
    public class PerScope : ILifetime
    {
        internal static readonly MethodInfo CreateScopedExpressionMethod = typeof(PerScope).GetRuntimeMethods().Single(x => x.Name == nameof(CreateScopedExpression));

        /// <inheritdoc />
        public void ApplyCaching(Scoped containerScope, ExpressionContext context)
        {
            MethodInfo method = CreateScopedExpressionMethod.MakeGenericMethod(context.Expression.Type);
            context.Expression = (Expression)method.Invoke(null, new object[] { context.Expression });
            context.ScopedExpressions.Clear();
        }

        public static Expression CreateScopedExpression<T>(Expression expression)
        {
            var factory = Expression.Lambda(expression, ExpressionGenerator.ScopeParameter).CompileFast<Func<Scoped, T>>();
            MethodInfo method = Scoped.GetOrAddScopedInstanceMethod.MakeGenericMethod(expression.Type);
            return Expression.Call(ExpressionGenerator.ScopeParameter, method, Expression.Constant(factory), Expression.Constant(expression.Type));
        }
    }
}