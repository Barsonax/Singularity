using System;

namespace Singularity.Exceptions
{
	public class InvalidExpressionArgumentsException : Exception
	{
		public InvalidExpressionArgumentsException(string message) : base(message)
		{
		}
	}
}