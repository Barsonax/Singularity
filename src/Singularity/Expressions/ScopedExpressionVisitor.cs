using System.Linq.Expressions;

namespace Singularity.Expressions
{
    public class ScopedExpressionVisitor : ExpressionVisitor
    {
        private readonly MethodCallExpression OldValue;
        private readonly Expression NewValue;

        public ScopedExpressionVisitor(MethodCallExpression oldValue, Expression newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.GetGenericMethodDefinition() == Scoped.GetOrAddScopedInstanceMethod && node.Type == OldValue.Type) return NewValue;
            return base.VisitMethodCall(node);
        }
    }
}
