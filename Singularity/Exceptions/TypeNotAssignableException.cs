using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public class TypeNotAssignableException : SingularityException
    {
        public TypeNotAssignableException()
        {
        }

        public TypeNotAssignableException(string message) : base(message)
        {
        }

        public TypeNotAssignableException(string message, Exception innerException) : base(message, innerException) { }

        protected TypeNotAssignableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}