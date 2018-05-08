using System;

namespace Singularity
{
	public enum Lifetime
	{
        /// <summary>
        /// Everytime the dependency is needed a new instance will be created.
        /// </summary>
		PerCall,

        /// <summary>
        /// Everytime the dependency is needed the same instance will be returned. <see cref="IDisposable"/> will be called when the container is disposed.
        /// </summary>
		PerContainer
    }
}