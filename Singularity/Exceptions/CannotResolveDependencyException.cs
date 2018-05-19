using System;
using System.Collections.Generic;
using System.Linq;

namespace Singularity.Exceptions
{
	public class CannotResolveDependencyException : Exception
	{
		public Type Type { get; }

		public CannotResolveDependencyException(Type type) : base($"Could not resolve dependency {type}")
		{
			Type = type;
		}
	}

	public class CannotResolveDependenciesException : Exception
	{
		public Type Type { get; }
		public Type[] UnresolvedTypes { get; }

		public CannotResolveDependenciesException(Type type, IEnumerable<Type> unresolvedTypes) : base($"Could not resolve the following dependencies for {type}: {string.Join(", ", unresolvedTypes)}")
		{
			Type = type;
			UnresolvedTypes = unresolvedTypes.ToArray();
		}
	}
}
