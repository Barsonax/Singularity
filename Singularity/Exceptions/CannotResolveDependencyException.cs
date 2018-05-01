using System;

namespace Singularity.Exceptions
{
    public class CannotResolveDependencyException : Exception
    {
		public CannotResolveDependencyException(string message) : base(message)
		{
		}
	}
}
