using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Singularity.FastExpressionCompiler;

namespace Singularity.Expressions
{
    internal class ExpressionCompiler
    {
        public static Func<Scoped, object> Compile(ReadOnlyExpressionContext context)
        {
            Expression expression = OptimizeExpression(context);
            return (Func<Scoped, object>)Expression.Lambda(expression, ExpressionGenerator.ScopeParameter).CompileFast();
        }

        public static Expression OptimizeExpression(ReadOnlyExpressionContext context)
        {
            Expression expression = context.Expression;
            if (context.ScopedExpressions.Count > 1)
            {
                var body = new List<Expression>();
                foreach (IGrouping<Type, MethodCallExpression> grouping in context.ScopedExpressions.GroupBy(x => x.Type))
                {
                    if (grouping.Count() > 1)
                    {
                        MethodCallExpression methodCallExpression = grouping.First();

                        ParameterExpression newValue = Expression.Variable(methodCallExpression.Type);
                        var visitor = new ScopedExpressionVisitor(methodCallExpression, newValue);
                        expression = visitor.Visit(expression);
                        body.Add(newValue);
                        body.Add(Expression.Assign(newValue, methodCallExpression));
                    }
                }

                if (body.Count > 0)
                {

                    expression = Expression.Block(body.Concat(new[] { expression }));
                }
            }
            return expression;
        }
    }
}
