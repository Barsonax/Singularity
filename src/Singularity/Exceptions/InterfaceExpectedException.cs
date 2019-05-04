using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    /// <summary>
    /// Exception for when a type should be a interface but its not.
    /// </summary>
    [Serializable]
    public sealed class InterfaceExpectedException : SingularityException
    {
        internal InterfaceExpectedException(string message, Exception? inner = null) : base(message, inner)
        {
        }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private InterfaceExpectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}