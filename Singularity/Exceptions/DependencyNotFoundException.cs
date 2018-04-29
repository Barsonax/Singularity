using System;

namespace Singularity.Exceptions
{
	public class DependencyNotFoundException : Exception
	{
		public DependencyNotFoundException(string message) : base(message)
		{
		}
	}
}
