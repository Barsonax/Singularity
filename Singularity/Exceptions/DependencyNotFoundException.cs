using System;

namespace Singularity.Exceptions
{
	public sealed class DependencyNotFoundException : SingularityException
    {
		public Type Type { get; }

		internal DependencyNotFoundException(Type type) : base($"Could not find dependency {type}")
		{
			Type = type;
		}
	}
}
