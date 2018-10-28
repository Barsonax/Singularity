using System;
using System.Collections.Generic;
using System.Linq;

namespace Singularity.Graph
{
	public class Graph<T>
		where T : class
	{
		private readonly Dictionary<T, Node<T>> _nodes = new Dictionary<T, Node<T>>();

		public Graph(IEnumerable<T> values)
		{
			Add(values);
		}

		public void Add(T value)
		{
			_nodes.Add(value, new Node<T>(value));
		}

		public void Add(IEnumerable<T> values)
		{
			foreach (var value in values)
			{
				Add(value);
			}
		}

		public T[][] GetUpdateOrder(Func<T, IEnumerable<T>> parentSelector)
		{
			if (_nodes.Values.TryExecute(node =>
			{
				node.Parents = parentSelector.Invoke(node.Value).Select(x => _nodes[x]).ToArray();
			}, out var parentSelectorExceptions))
			{
				throw new SingularityAggregateException("The following exceptions occured while determining the parents of the nodes in the graph", parentSelectorExceptions);
			}

			if (_nodes.Values.Where(x => x.Parents.Length != 0).TryExecute(node =>
			{
				node.Depth = ResolveDepth(node);
			}, out var depthCalculationExceptions))
			{
				throw new SingularityAggregateException("The following exceptions occured while calculating the depth of the nodes in the graph", depthCalculationExceptions);
			}

			return _nodes.Values.GroupBy(x => x.Depth).OrderBy(x => x.Key).Select(x => x.Select(y => y.Value).ToArray()).ToArray();
		}

		private int ResolveDepth(Node<T> dependencyNode, List<Node<T>> visitedNodes = null)
		{
			if (visitedNodes == null)
			{
				visitedNodes = new List<Node<T>>();
			}
			else if (visitedNodes.Contains(dependencyNode))
			{
				visitedNodes.Add(dependencyNode);
				throw new CircularDependencyException(visitedNodes.Select(x => x.Value).ToArray());
			}

			visitedNodes.Add(dependencyNode);

			var maxDepth = 0;

			foreach (var parent in dependencyNode.Parents)
			{
				if (parent.Depth == null)
					parent.Depth = ResolveDepth(parent, visitedNodes);
				var currentDepth = parent.Depth.Value + 1;
				if (currentDepth > maxDepth) maxDepth = currentDepth;
			}
			return maxDepth;
		}
	}

	public class Node<T>
		where T : class
	{
		public int? Depth { get; set; }
		public Node<T>[] Parents { get; set; }
		public T Value { get; }

		public Node(T value)
		{
			Value = value;
		}
	}
}