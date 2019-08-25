using System;
using Singularity.Expressions;

namespace Singularity
{
    /// <summary>
    /// The same instance will be used in the entire graph
    /// Not implemented
    /// </summary>
    public class PerGraph : ILifetime
    {
        /// <inheritdoc />
        public void ApplyCaching(Scoped containerScope, ExpressionContext context)
        {
            throw new NotImplementedException(nameof(PerGraph));
        }
    }
}