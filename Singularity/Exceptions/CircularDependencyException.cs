using System;

namespace Singularity
{
	public class CircularDependencyException : Exception
	{
		public CircularDependencyException(string message) : base(message)
		{
		}
	}
}