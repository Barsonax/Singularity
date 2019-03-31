using System.Collections.Generic;

namespace Singularity
{
    /// <summary>
    /// Specifies when new instances of dependencies should be made
    /// </summary>
    public enum CreationMode
    {
        /// <summary>
        /// Every time a dependency is requested a new instance is returned
        /// </summary>
        Transient,
        /// <summary>
        /// Every time a dependency is requested the same instance is returned
        /// </summary>
        Singleton
    }
}