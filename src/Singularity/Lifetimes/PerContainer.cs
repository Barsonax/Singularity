using System;
using System.Linq.Expressions;
using Singularity.Expressions;
using Singularity.FastExpressionCompiler;

namespace Singularity
{
    /// <summary>
    /// The same instance will be returned as long as it is requested in the same <see cref="Container"/> or a child of this container.
    /// </summary>
    public class PerContainer : ILifetime
    {
        /// <inheritdoc />
        public void ApplyCaching(Scoped containerScope, ExpressionContext context)
        {
            object singletonInstance = GetSingleton(containerScope, context.Expression);
            context.Expression = Expression.Constant(singletonInstance, context.Expression.Type);
            context.ScopedExpressions.Clear();
        }

        private static object GetSingleton(Scoped containerScope, Expression expression)
        {
            switch (expression)
            {
                case ConstantExpression constantExpression:
                    return constantExpression.Value;
                case NewExpression newExpression:
                    if (newExpression.Arguments.Count == 0)
                    {
                        return newExpression.Constructor.Invoke(null);
                    }
                    else
                    {
                        return ((Func<Scoped, object>)Expression.Lambda(expression, ExpressionGenerator.ScopeParameter).CompileFast())(containerScope);
                    }
                default:
                    return ((Func<Scoped, object>)Expression.Lambda(expression, ExpressionGenerator.ScopeParameter).CompileFast())(containerScope);
            }
        }
    }
}