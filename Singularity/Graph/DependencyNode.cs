using System.Linq.Expressions;

namespace Singularity
{
    public class DependencyNode
	{
        public Expression Expression { get; internal set; }
        public Lifetime Lifetime { get; }

        public DependencyNode(Expression expression, Lifetime lifetime)
        {
            Lifetime = lifetime;
            Expression = expression;
        }

        public override string ToString()
        {
            return $"Type {Expression.Type} Lifetime {Lifetime}";
        }
    }
}