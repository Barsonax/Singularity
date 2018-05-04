using System;

namespace Singularity
{
	internal class InvalidExpressionArgumentsException : Exception
	{
		public InvalidExpressionArgumentsException()
		{
		}

		public InvalidExpressionArgumentsException(string message) : base(message)
		{
		}

		public InvalidExpressionArgumentsException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}