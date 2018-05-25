using System.Collections.Generic;
using System.Linq.Expressions;
using Singularity.Bindings;

namespace Singularity.Graph
{
    public interface IDependencyExpressionGenerator
    {
        void Generate(IBinding binding, ParameterExpression instanceParameter, List<ParameterExpression> parameters, List<Expression> body);
    }
}