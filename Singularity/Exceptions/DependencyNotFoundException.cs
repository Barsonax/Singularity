using System;

namespace Singularity.Exceptions
{
	public class DependencyNotFoundException : Exception
	{
		public Type Type { get; }

		public DependencyNotFoundException(Type type) : base($"Could not find dependency {type}")
		{
			Type = type;
		}
	}
}
