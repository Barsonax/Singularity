using System;
using System.Collections.Generic;
using System.Linq;

namespace Singularity.Exceptions
{
	public class DependenciesNotFoundException : Exception
	{
		public Type Type { get; }
		public Type[] MissingDependencies { get; }

		public DependenciesNotFoundException(Type type, IEnumerable<Type> missingDependencies) : base($"Could not find the following dependencies for {type}: {string.Join(", ", missingDependencies)}")
		{
			Type = type;
			MissingDependencies = missingDependencies.ToArray();
		}
	}
}
