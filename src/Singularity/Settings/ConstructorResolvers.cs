using Singularity.Resolvers;

namespace Singularity
{
    /// <summary>
    /// Provides easy access to build in constructor selectors.
    /// </summary>
    public static class ConstructorResolvers
    {
        /// <summary>
        /// The default strict constructor injector
        /// </summary>
        public static IConstructorResolver Default { get; } = new ConstructorResolverCache(new DefaultConstructorResolver());

        /// <summary>
        /// A resolver that picks the constructor with the most arguments that can still be resolved.
        /// </summary>
        public static IConstructorResolver BestMatch { get; } = new ConstructorResolverCache(new BestMatchConstructorResolver());
    }
}
