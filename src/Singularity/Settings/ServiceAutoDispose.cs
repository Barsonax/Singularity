using System;

namespace Singularity
{
    /// <summary>
    /// Specifies if a service should be auto disposed.
    /// </summary>
    public enum ServiceAutoDispose
    {
        /// <summary>
        /// Will dispose a instance if its a <see cref="IDisposable"/> and <see cref="SingularitySettings.AutoDispose"/> is true.
        /// </summary>
        Default,
        /// <summary>
        /// Never disposes a instance regardless of the value of <see cref="SingularitySettings.AutoDispose"/>.
        /// </summary>
        Never,
        /// <summary>
        /// Will always dispose a instance if its a <see cref="IDisposable"/> regardless of the value of <see cref="SingularitySettings.AutoDispose"/>.
        /// </summary>
        Always
    }
}