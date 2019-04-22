using System.Linq;
using System.Linq.Expressions;
using Singularity.Graph;

namespace Singularity.Expressions
{
    internal sealed class ParameterExpressionVisitor : ExpressionVisitor
    {
        private readonly InstanceFactory[] _factories;
        public ParameterExpressionVisitor(InstanceFactory[] factories)
        {
            _factories = factories;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node.Type == typeof(Scoped)) return ExpressionGenerator.ScopeParameter;
            InstanceFactory factory = _factories.First(x => x.DependencyType == node.Type);

            return factory.Expression!;
        }
    }
}