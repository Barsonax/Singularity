using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public class NoConstructorException : SingularityException
    {
        public NoConstructorException()
        {
        }

        public NoConstructorException(string message) : base(message)
        {
        }

        public NoConstructorException(string message, Exception inner) : base(message, inner)
        {
        }

        protected NoConstructorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}