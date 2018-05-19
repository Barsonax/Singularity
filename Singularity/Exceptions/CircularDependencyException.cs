using System;
using System.Collections.Generic;
using System.Linq;

namespace Singularity
{
	public class CircularDependencyException : Exception
	{
		
		public IReadOnlyCollection<object> VisitedNodes { get; }

		public CircularDependencyException(IReadOnlyCollection<object> visitedNodes) : base($"Node {visitedNodes.First()} has circular dependencies! ({string.Join("->", visitedNodes)})")
		{
			VisitedNodes = visitedNodes;
		}
	}
}