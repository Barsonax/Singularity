using System;
using System.Collections.Generic;
using System.Text;

namespace Singularity.Exceptions
{
    public class CannotResolveDependencyException : Exception
    {
		public CannotResolveDependencyException(string message) : base(message)
		{
		}
	}
}
