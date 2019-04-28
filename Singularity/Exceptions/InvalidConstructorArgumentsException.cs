using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    /// <summary>
    /// Exception for when the arguments of a exception are incorrect.
    /// </summary>
    [Serializable]
    public class InvalidExpressionArgumentsException : SingularityException
    {
        internal InvalidExpressionArgumentsException(string message, Exception? inner = null) : base(message, inner)
        {
        }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected InvalidExpressionArgumentsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}