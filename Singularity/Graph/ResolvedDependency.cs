using System;
using System.Linq.Expressions;

namespace Singularity.Graph
{
    internal sealed class ResolvedDependency
    {
        public Expression Expression { get; }
        public ILifetime Lifetime { get; }

        public ResolvedDependency(Expression expression, ILifetime lifetime)
        {
	        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            Lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
        }
    }
}