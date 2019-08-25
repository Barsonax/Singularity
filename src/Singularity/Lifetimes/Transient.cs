using Singularity.Expressions;

namespace Singularity
{
    /// <summary>
    /// Every time a instance is requested a new instance is returned.
    /// </summary>
    public class Transient : ILifetime
    {
        /// <inheritdoc />
        public void ApplyCaching(Scoped containerScope, ExpressionContext context)
        {
            //No caching for transients.
        }
    }
}