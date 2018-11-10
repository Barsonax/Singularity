using System;
using System.Linq.Expressions;

namespace Singularity.Graph
{
    public sealed class ResolvedDependency
    {
        public Expression Expression { get; }

        public ResolvedDependency(Expression expression)
        {
	        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }
    }
}