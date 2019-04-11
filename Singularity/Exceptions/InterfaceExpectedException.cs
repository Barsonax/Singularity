using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public class InterfaceExpectedException : SingularityException
    {
        public InterfaceExpectedException()
        {
        }

        internal InterfaceExpectedException(string message) : base(message)
        {
        }

        public InterfaceExpectedException(string message, Exception inner) : base(message, inner)
        {
        }


        protected InterfaceExpectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}