using System;
using System.Linq.Expressions;

namespace Singularity
{
    public class DependencyNode
	{
        public Expression Expression { get; internal set; }
        public Lifetime Lifetime { get; }
        public Action<object> OnDeath { get; }
        public bool IsExternal { get; }

	    public DependencyNode(Expression expression, Lifetime lifetime, Action<object> onDeath, bool isExternal = false)
        {
            Lifetime = lifetime;
            Expression = expression;
            OnDeath = onDeath;
            IsExternal = isExternal;
        }
    }
}