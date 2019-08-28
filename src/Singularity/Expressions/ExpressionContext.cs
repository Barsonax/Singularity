using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Singularity.Expressions
{
    /// <summary>
    /// Contains useful info about a expression.
    /// </summary>
    public sealed class ExpressionContext
    {
        /// <summary>
        /// The expressions in <see cref="Expression"/> that come from <see cref="Scoped"/>
        /// </summary>
        public List<MethodCallExpression> ScopedExpressions { get; }

        /// <summary>
        /// The expression to create the instance.
        /// </summary>
        public Expression Expression { get; set; }

        internal ExpressionContext(Expression expression)
        {
            ScopedExpressions = new List<MethodCallExpression>();
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        internal ExpressionContext(ReadOnlyExpressionContext context)
        {
            ScopedExpressions = context.ScopedExpressions.ToList();
            Expression = context.Expression ?? throw new ArgumentNullException("context.Expression");
        }

        public static implicit operator ReadOnlyExpressionContext(ExpressionContext context)
        {
            return new ReadOnlyExpressionContext(context);
        }
    }

    /// <summary>
    /// Same as <see cref="ExpressionContext"/> but readonly.
    /// </summary>
    public sealed class ReadOnlyExpressionContext
    {
        /// <summary>
        /// <see cref="ExpressionContext.ScopedExpressions"/>
        /// </summary>
        public ReadOnlyCollection<MethodCallExpression> ScopedExpressions { get; }
        internal Expression Expression { get; }

        internal ReadOnlyExpressionContext(ExpressionContext context)
        {
            ScopedExpressions = new ReadOnlyCollection<MethodCallExpression>(context.ScopedExpressions.ToArray());
            Expression = context.Expression ?? throw new ArgumentNullException("context.Expression");
        }

        public static explicit operator ExpressionContext(ReadOnlyExpressionContext context)
        {
            return new ExpressionContext(context);
        }
    }
}
