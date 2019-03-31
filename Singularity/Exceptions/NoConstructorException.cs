using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public sealed class NoConstructorException : SingularityException
    {
        internal NoConstructorException(string message) : base(message)
        {
        }

        public NoConstructorException()
        {
        }

        protected NoConstructorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}