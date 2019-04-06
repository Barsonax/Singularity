using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public class CannotAutoResolveConstructorException : SingularityException
    {
        internal CannotAutoResolveConstructorException(string message) : base(message)
        {
        }

        public CannotAutoResolveConstructorException()
        {
        }

        protected CannotAutoResolveConstructorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}