using System.Linq.Expressions;

namespace Singularity.Expressions
{
    internal sealed class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private Expression? _oldValue;
        private Expression? _newValue;


        public Expression Visit(Expression node, Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
            return Visit(node);
        }

        public override Expression Visit(Expression node)
        {
            return node == _oldValue! ? _newValue! : base.Visit(node);
        }
    }
}