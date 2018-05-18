using System;
using System.Linq.Expressions;

namespace Singularity
{
    public class DependencyNode
    {
        public UnresolvedDependency UnresolvedDependency { get; }
        public ResolvedDependency ResolvedDependency { get; internal set; }

        public DependencyNode(UnresolvedDependency unresolvedDependency)
        {
            UnresolvedDependency = unresolvedDependency;
        }
    }

    public sealed class UnresolvedDependency
	{
        public Expression Expression { get; internal set; }
        public Lifetime Lifetime { get; }
        public Action<object> OnDeath { get; internal set; }

	    public UnresolvedDependency(Expression expression, Lifetime lifetime, Action<object> onDeath)
        {
            Lifetime = lifetime;
            Expression = expression;
            OnDeath = onDeath;
        }
	}

    public class ResolvedDependency
    {
        public Expression Expression { get; }

        public ResolvedDependency(Expression expression)
        {
            Expression = expression;
        }
    }
}