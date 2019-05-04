using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    /// <summary>
    /// Thrown if a type is not assignable to another type.
    /// </summary>
    [Serializable]
    public sealed class TypeNotAssignableException : SingularityException
    {
        internal TypeNotAssignableException(string message, Exception? innerException = null) : base(message, innerException) { }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private TypeNotAssignableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}