using System.Collections;
using System.Collections.Generic;

namespace Singularity.TestClasses.TestClasses
{
	public class TestNode
	{
		public string Name { get; }
		public List<TestNode> Parents { get; }

		public TestNode(string name)
		{
			Name = name;
			Parents = new List<TestNode>();
		}

		public override string ToString()
		{
			return Name;
		}
	}

	public class TestNodeCollection : IEnumerable<TestNode>
	{
		private readonly List<TestNode> _nodes = new List<TestNode>();

		public TestNode Add(string name)
		{
			var node = new TestNode(name);
			_nodes.Add(node);
			return node;
		}

		public IEnumerator<TestNode> GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
