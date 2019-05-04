using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    /// <summary>
    /// Exception for when the arguments of a exception are incorrect.
    /// </summary>
    [Serializable]
    public sealed class InvalidExpressionArgumentsException : SingularityException
    {
        internal InvalidExpressionArgumentsException(string message, Exception? inner = null) : base(message, inner)
        {
        }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private InvalidExpressionArgumentsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}