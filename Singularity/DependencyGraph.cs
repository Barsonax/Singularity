using System;
using System.Collections.Generic;
using System.Linq;

namespace Singularity
{
	public class DependencyGraph
	{
		public Dictionary<Type, DependencyNode> Nodes { get; }
		public IReadOnlyList<IReadOnlyList<DependencyNode>> UpdateOrder => _updateOrder;

		private readonly List<List<DependencyNode>> _updateOrder;

		public DependencyGraph(IEnumerable<IBinding> bindings)
		{
			Nodes = new Dictionary<Type, DependencyNode>();
			foreach (var binding in bindings)
			{
				var node = new DependencyNode(binding.DependencyType, binding.BindingExpression);
				Nodes.Add(binding.DependencyType, node);
			}

			foreach (var dependencyNode in Nodes.Values)
			{
				foreach (var parameterExpression in dependencyNode.Dependencies)
				{
					if (Nodes.TryGetValue(parameterExpression.Parameter.Type, out var parent))
					{
						parameterExpression.Dependency = parent;
					}
				}
			}

			var unresolvedNodes = Nodes.Values.Where(x => x.Dependencies.Length != 0).ToList();

			foreach (var unresolvedNode in unresolvedNodes)
			{
				unresolvedNode.Depth = ResolveDepth(unresolvedNode);
			}

			_updateOrder = Nodes.Values.GroupBy(x => x.Depth).OrderBy(x => x.Key).Select(x => x.ToList()).ToList();
		}

		private int ResolveDepth(DependencyNode dependencyNode)
		{
			var maxDepth = 0;
			foreach (var parameterDependency in dependencyNode.Dependencies)
			{
				if (parameterDependency.Dependency.Depth == null)
					parameterDependency.Dependency.Depth = ResolveDepth(parameterDependency.Dependency);
				var currentDepth = parameterDependency.Dependency.Depth.Value + 1;
				if (currentDepth > maxDepth) maxDepth = currentDepth;
			}
			return maxDepth;
		}
	}
}