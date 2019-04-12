using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public class InterfaceExpectedException : SingularityException
    {
        internal InterfaceExpectedException(string message, Exception? inner = null) : base(message, inner)
        {
        }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected InterfaceExpectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}