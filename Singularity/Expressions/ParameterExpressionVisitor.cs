using System.Linq;
using System.Linq.Expressions;
using Singularity.Graph;

namespace Singularity.Expressions
{
    internal sealed class ParameterExpressionVisitor : ExpressionVisitor
    {
        private readonly Dependency[] _dependencies;
        public ParameterExpressionVisitor(Dependency[] dependencies)
        {
            _dependencies = dependencies;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            var nestedDependency = _dependencies.First(x => x.Binding.DependencyType == node.Type);

            return nestedDependency.Expression!;
        }
    }
}