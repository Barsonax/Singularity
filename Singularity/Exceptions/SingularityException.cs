using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
	public abstract class SingularityException : Exception
	{

		public SingularityException(){}

		internal SingularityException(string message) : base(message){}

		internal SingularityException(string message, Exception innerException) : base(message, innerException) { }

        protected SingularityException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}