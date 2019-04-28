using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    /// <summary>
    /// Exception for when no public constructor is available to be resolved.
    /// </summary>
    [Serializable]
    public class NoConstructorException : SingularityException
    {
        internal NoConstructorException(string message, Exception? inner = null) : base(message, inner)
        {
        }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected NoConstructorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}