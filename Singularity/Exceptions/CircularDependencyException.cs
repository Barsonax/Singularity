using System;
using System.Collections.Generic;
using System.Linq;

namespace Singularity.Exceptions
{
	public sealed class CircularDependencyException : SingularityException
    {
		public IReadOnlyList<Type> Cycle { get; }

		internal CircularDependencyException(IReadOnlyList<Type> cycle) : base($"{cycle.First()} has circular dependencies! ({string.Join("->", cycle.Select(x => x.Name))})")
		{
			Cycle = cycle;
		}
	}
}