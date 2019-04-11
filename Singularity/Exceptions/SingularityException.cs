using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
	public class SingularityException : Exception
	{

		public SingularityException(){}

        public SingularityException(string message) : base(message){}

		public SingularityException(string message, Exception innerException) : base(message, innerException) { }

        protected SingularityException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}