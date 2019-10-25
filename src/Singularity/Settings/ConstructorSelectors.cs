namespace Singularity
{
    /// <summary>
    /// Provides easy access to build in constructor selectors.
    /// </summary>
    public static class ConstructorSelectors
    {
        /// <summary>
        /// The default strict constructor injector
        /// </summary>
        public static DefaultConstructorResolver Default { get; } = new DefaultConstructorResolver();

        /// <summary>
        /// A selector that picks the constructor with the most arguments.
        /// </summary>
        public static MultipleConstructorResolver Multiple { get; } = new MultipleConstructorResolver();
    }
}
