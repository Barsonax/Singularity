using System.Linq.Expressions;

namespace Singularity.Graph
{
    public class ResolvedDependency
    {
        public Expression Expression { get; }

        public ResolvedDependency(Expression expression)
        {
            Expression = expression;
        }
    }
}