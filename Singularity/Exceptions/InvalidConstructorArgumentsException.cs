using System;

namespace Singularity.Exceptions
{
    [Serializable]
    public sealed class InvalidExpressionArgumentsException : SingularityException
    {
		internal InvalidExpressionArgumentsException(string message) : base(message)
		{
		}
	}
}