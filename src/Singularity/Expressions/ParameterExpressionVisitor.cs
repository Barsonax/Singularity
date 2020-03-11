using System.Linq;
using System.Linq.Expressions;

using Singularity.Resolving;

namespace Singularity.Expressions
{
    internal sealed class ParameterExpressionVisitor : ExpressionVisitor
    {
        private readonly InstanceFactory[] _factories;
        private readonly ExpressionContext _context;
        public ParameterExpressionVisitor(ExpressionContext context, InstanceFactory[] factories)
        {
            _context = context;
            _factories = factories;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node.Type == typeof(Scoped)) return ExpressionGenerator.ScopeParameter;
            InstanceFactory factory = _factories.First(x => x.DependencyType == node.Type);
            if (factory.Context.Expression is MethodCallExpression methodCallExpression && methodCallExpression.Method.IsGenericMethod && methodCallExpression.Method.GetGenericMethodDefinition() == Scoped.GetOrAddScopedInstanceMethod)
            {
                _context.ScopedExpressions.Add(methodCallExpression);
            }
            else
            {
                foreach (MethodCallExpression scopedExpression in factory.Context.ScopedExpressions)
                {
                    _context.ScopedExpressions.Add(scopedExpression);
                }
            }
            return factory.Context.Expression;
        }
    }
}