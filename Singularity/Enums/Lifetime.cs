using System;

namespace Singularity.Enums
{
    /// <summary>
    /// Specifies the lifetime of a instance
    /// </summary>
	public enum Lifetime
	{
        /// <summary>
        /// Everytime the instance is needed a new instance will be returned.
        /// </summary>
		PerCall,

        /// <summary>
        /// Everytime the instance is needed the same instance will be returned. <see cref="IDisposable"/> will be called when the container is disposed.
        /// </summary>
		PerContainer
    }
}