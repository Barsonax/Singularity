using System;
using Singularity.Collections;
using Singularity.Expressions;

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
            AutoDisposeLifetimes = new ILifetime[]
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
        public LifetimeCollection AutoDisposeLifetimes { get; set; } = LifetimeCollection.Empty;

        /// <summary>
        /// The constructor selector that will be used by default.
        /// </summary>
        public IConstructorSelector ConstructorSelector { get; set; } = ConstructorSelectors.Default;
    }
}
