using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public class CannotAutoResolveConstructorException : SingularityException
    {
        internal CannotAutoResolveConstructorException(string message, Exception? inner = null) : base(message, inner)
        {
        }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected CannotAutoResolveConstructorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}