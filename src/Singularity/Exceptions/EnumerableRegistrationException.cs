using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    /// <summary>
    /// Exception for when a enumerable is registered incorrectly.
    /// </summary>
    [Serializable]
    public sealed class EnumerableRegistrationException : Exception
    {
        internal EnumerableRegistrationException(string message, Exception? inner = null) : base(message, inner)
        {
        }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private EnumerableRegistrationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}