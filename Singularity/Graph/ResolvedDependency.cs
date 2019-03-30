using System;
using System.Linq.Expressions;

namespace Singularity.Graph
{
    internal sealed class ResolvedDependency
    {
        public Expression Expression { get; }
        public Func<object> InstanceFactory { get; }

        public ResolvedDependency(Expression expression, Func<object> instanceFactory)
        {
	        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            InstanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
        }
    }
}