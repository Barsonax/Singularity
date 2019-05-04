using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    /// <summary>
    /// Exception for when a constructor cannot be auto resolved.
    /// </summary>
    [Serializable]
    public sealed class CannotAutoResolveConstructorException : SingularityException
    {
        internal CannotAutoResolveConstructorException(string message, Exception? inner = null) : base(message, inner)
        {
        }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private CannotAutoResolveConstructorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}