using System;

namespace Singularity
{
	public class InvalidExpressionArgumentsException : Exception
	{
		public InvalidExpressionArgumentsException(string message) : base(message)
		{
		}
	}
}