using System;

namespace Singularity.Exceptions
{
	public abstract class SingularityException : Exception
	{

		internal SingularityException(){}

		internal SingularityException(string message) : base(message){}

		internal SingularityException(string message, Exception innerException) : base(message, innerException) { }
	}
}