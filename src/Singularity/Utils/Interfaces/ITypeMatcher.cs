using System;

namespace Singularity
{
    /// <summary>
    /// Interface for matching a type
    /// </summary>
    public interface ITypeMatcher
    {
        /// <summary>
        /// Returns true if the type matches.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool Match(Type type);
    }
}