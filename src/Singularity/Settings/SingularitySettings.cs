using System;

namespace Singularity
{
    /// <summary>
    /// Defines settings for the singularity container.
    /// </summary>
    public sealed class SingularitySettings
    {
        /// <summary>
        /// The default singularity settings.
        /// </summary>
        public static readonly SingularitySettings Default = new SingularitySettings();

        /// <summary>
        /// Settings for microsoft dependency injection.
        /// </summary>
        public static readonly SingularitySettings Microsoft = new SingularitySettings
        {
            AutoDispose = new ILifetime[]
            {
                Lifetimes.Transient,
                Lifetimes.PerContainer,
                Lifetimes.PerScope,
                Lifetimes.PerGraph,
            }
        };

        /// <summary>
        /// Specifies what lifetimes should be auto disposed if the instance is a <see cref="IDisposable"/> and the service is registered with <see cref="ServiceAutoDispose.Default"/>.
        /// </summary>
        public LifetimeCollection AutoDispose { get; set; } = LifetimeCollection.Empty;
    }
}
