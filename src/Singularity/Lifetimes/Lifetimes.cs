namespace Singularity
{
    /// <summary>
    /// Specifies when new instances of dependencies should be made
    /// </summary>
    public static class Lifetimes
    {
        /// <summary>
        /// Every time a instance is requested a new instance is returned.
        /// </summary>
        public static Transient Transient { get; } = new Transient();
        /// <summary>
        /// The same instance will be returned as long as it is requested in the same <see cref="Container"/> or a child of this container.
        /// </summary>
        public static PerContainer PerContainer { get; } = new PerContainer();
        /// <summary>
        /// The same instance will be returned as long as it is requested in the same <see cref="Scoped"/>.
        /// </summary>
        public static PerScope PerScope { get; } = new PerScope();
        /// <summary>
        /// The same instance will be used in the entire graph
        /// Not implemented
        /// </summary>
        public static PerGraph PerGraph { get; } = new PerGraph();
    }
}