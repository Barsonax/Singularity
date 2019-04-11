using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public class InvalidExpressionArgumentsException : SingularityException
    {
		internal InvalidExpressionArgumentsException(string message) : base(message)
		{
		}

        public InvalidExpressionArgumentsException()
        {
        }

        public InvalidExpressionArgumentsException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidExpressionArgumentsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}