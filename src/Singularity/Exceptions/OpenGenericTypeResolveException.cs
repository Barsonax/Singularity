using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    /// <summary>
    /// Exception thrown when trying to create a instance for a open generic type.
    /// </summary>
    [Serializable]
    public sealed class OpenGenericTypeResolveException : Exception
    {
        internal OpenGenericTypeResolveException(string message, Exception? inner = null) : base(message, inner)
        {
        }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private OpenGenericTypeResolveException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
