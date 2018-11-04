using System;

namespace Singularity.Exceptions
{
	public abstract class SingularityException : Exception
	{

		public SingularityException(){}

		public SingularityException(string message) : base(message){}

		public SingularityException(string message, Exception innerException) : base(message, innerException) { }
	}
}