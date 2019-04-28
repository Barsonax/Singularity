using System;

namespace Singularity
{
    /// <summary>
    /// Specifies when instances should be disposed.
    /// </summary>
    public enum DisposeBehavior
    {
        /// <summary>
        /// Will dispose a instance if its a <see cref="IDisposable"/> and <see cref="SingularitySettings.AutoDispose"/> is true.
        /// </summary>
        Default,
        /// <summary>
        /// Never disposes a instance.
        /// </summary>
        Never,
        /// <summary>
        /// Will always dispose a instance if its a <see cref="IDisposable"/>
        /// </summary>
        Always
    }
}