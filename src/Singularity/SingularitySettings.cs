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
        public static readonly SingularitySettings Microsoft = new SingularitySettings { AutoDispose = true };

        /// <summary>
        /// If true then <see cref="IDisposable"/> instances will automatically be disposed by the container.
        /// </summary>
        public bool AutoDispose { get; set; }
    }
}
