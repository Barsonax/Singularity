using System;

namespace Singularity.Exceptions
{
	public sealed class InvalidExpressionArgumentsException : SingularityException
    {
		internal InvalidExpressionArgumentsException(string message) : base(message)
		{
		}
	}
}