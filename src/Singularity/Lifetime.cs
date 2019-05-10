namespace Singularity
{
    /// <summary>
    /// Specifies when new instances of dependencies should be made
    /// </summary>
    public enum Lifetime
    {
        /// <summary>
        /// Every time a instance is requested a new instance is returned.
        /// </summary>
        Transient,
        /// <summary>
        /// The same instance will be returned as long as it is requested in the same <see cref="Container"/> or a child of this container.
        /// </summary>
        PerContainer,
        /// <summary>
        /// The same instance will be returned as long as it is requested in the same <see cref="Scoped"/>.
        /// </summary>
        PerScope,
        /// <summary>
        /// The same instance will be used in the entire graph
        /// </summary>
        PerGraph
    }
}