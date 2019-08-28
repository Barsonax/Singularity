using Singularity.Expressions;

namespace Singularity
{
    /// <summary>
    /// Defines a lifetime.
    /// </summary>
    public interface ILifetime
    {
        /// <summary>
        /// Applies caching on the expression.
        /// </summary>
        /// <param name="containerScope"></param>
        /// <param name="context"></param>
        void ApplyCaching(Scoped containerScope, ExpressionContext context);
    }
}