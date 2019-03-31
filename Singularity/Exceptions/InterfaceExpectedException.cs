using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public sealed class InterfaceExpectedException : SingularityException
    {
	    internal InterfaceExpectedException(string message) : base(message)
        {
        }

        public InterfaceExpectedException()
        {
        }

        protected InterfaceExpectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}