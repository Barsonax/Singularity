using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public class TypeNotAssignableException : SingularityException
    {
        internal TypeNotAssignableException(string message) : base(message)
        {
        }

        public TypeNotAssignableException()
        {
        }

        protected TypeNotAssignableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}