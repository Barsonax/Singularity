using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public class CannotAutoResolveConstructorException : SingularityException
    {
        public CannotAutoResolveConstructorException()
        {
        }

        internal CannotAutoResolveConstructorException(string message) : base(message)
        {
        }

        public CannotAutoResolveConstructorException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CannotAutoResolveConstructorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}