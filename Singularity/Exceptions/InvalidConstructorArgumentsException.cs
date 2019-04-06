using System;

namespace Singularity.Exceptions
{
    [Serializable]
    public class InvalidExpressionArgumentsException : SingularityException
    {
		internal InvalidExpressionArgumentsException(string message) : base(message)
		{
		}
	}
}