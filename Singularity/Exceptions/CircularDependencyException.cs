using System.Collections.Generic;
using System.Linq;

namespace Singularity.Exceptions
{
	public sealed class CircularDependencyException : SingularityException
    {
		public IReadOnlyCollection<object> VisitedNodes { get; }

		internal CircularDependencyException(IReadOnlyCollection<object> visitedNodes) : base($"{visitedNodes.First()} has circular dependencies! ({string.Join("->", visitedNodes)})")
		{
			VisitedNodes = visitedNodes;
		}
	}
}