using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Singularity.Graph
{
    internal sealed class Dependency
    {
        public Binding Binding { get; }
        public Dependency[]? Dependencies { get; set; }
        public ResolvedDependency? ResolvedDependency { get; set; }

        public Dependency(Binding unresolvedDependency)
        {
            Binding = unresolvedDependency;
        }

        public IEnumerable<ParameterExpression> GetParameters()
        {
            return Binding.Expression.GetParameterExpressions();
        }

        public IEnumerable<ParameterExpression> GetDecoratorParameters()
        {
            return Binding.Decorators.SelectMany(x => x.GetParameterExpressions()).Where(x => x.Type != Binding.DependencyType);
        }
    }
}