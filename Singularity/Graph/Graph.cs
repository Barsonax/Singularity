using System;
using System.Collections.Generic;
using System.Linq;

namespace Singularity.Graph
{
    public class Graph<T>
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
            foreach (var node in _nodes.Values)
            {
                node.Parents = parentSelector.Invoke(node.Value).Select(x => _nodes[x]).ToArray();
            }

            var unresolvedNodes = _nodes.Values.Where(x => x.Parents.Length != 0).ToList();

            foreach (var unresolvedNode in unresolvedNodes)
            {
                unresolvedNode.Depth = ResolveDepth(unresolvedNode, unresolvedNode, true);
            }

            return _nodes.Values.GroupBy(x => x.Depth).OrderBy(x => x.Key).Select(x => x.Select(y => y.Value).ToArray()).ToArray();
        }

        private int ResolveDepth(Node<T> dependencyNode, Node<T> startNode, bool isRootCall = false)
        {
            if (!isRootCall && dependencyNode == startNode) throw new CircularDependencyException();
            var maxDepth = 0;

            foreach (var parent in dependencyNode.Parents)
            {
                if (parent.Depth == null)
                    parent.Depth = ResolveDepth(parent, startNode);
                var currentDepth = parent.Depth.Value + 1;
                if (currentDepth > maxDepth) maxDepth = currentDepth;
            }
            return maxDepth;
        }
    }

    public class Node<T>
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