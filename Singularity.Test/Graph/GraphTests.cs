using System;
using System.Linq;
using Singularity.Graph;
using Xunit;

namespace Singularity.Test
{
	public class GraphTests
	{
		[Fact]
		public void CheckUpdateOrder_Simple()
		{
			var nodeCollection = new TestNodeCollection();

			var node1 = nodeCollection.Add("node1");
			var node2 = nodeCollection.Add("node2");
			var node3 = nodeCollection.Add("node3");

			node3.Parents.Add(node2);
			node2.Parents.Add(node1);

			var graph = new Graph<TestNode>(nodeCollection);
			var updateOrder = graph.GetUpdateOrder(node => node.Parents);

			Assert.Equal(3, updateOrder.Length);

			Assert.Equal(1, updateOrder[0].Length);
			Assert.Equal(updateOrder[0][0], node1);

			Assert.Equal(1, updateOrder[1].Length);
			Assert.Equal(updateOrder[1][0], node2);

			Assert.Equal(1, updateOrder[2].Length);
			Assert.Equal(updateOrder[2][0], node3);
		}

		[Fact]
		public void CheckUpdateOrder_Complex()
		{
			var nodeCollection = new TestNodeCollection();

			var node1 = nodeCollection.Add("node1");
			var node2 = nodeCollection.Add("node2");
			var node3 = nodeCollection.Add("node3");
			var node4 = nodeCollection.Add("node4");

			node2.Parents.Add(node1);
			node3.Parents.Add(node2);
			node4.Parents.Add(node1);
			node4.Parents.Add(node2);
			node4.Parents.Add(node3);

			var graph = new Graph<TestNode>(nodeCollection);
			var updateOrder = graph.GetUpdateOrder(node => node.Parents);

			Assert.Equal(4, updateOrder.Length);

			Assert.Equal(1, updateOrder[0].Length);
			Assert.Equal(updateOrder[0][0], node1);

			Assert.Equal(1, updateOrder[1].Length);
			Assert.Equal(updateOrder[1][0], node2);

			Assert.Equal(1, updateOrder[2].Length);
			Assert.Equal(updateOrder[2][0], node3);

			Assert.Equal(1, updateOrder[3].Length);
			Assert.Equal(updateOrder[3][0], node4);
		}

		[Fact]
		public void CircularDependency_Simple_Throws()
		{
			var nodeCollection = new TestNodeCollection();

			var node1 = nodeCollection.Add("node1");
			var node2 = nodeCollection.Add("node2");

			node1.Parents.Add(node2);
			node2.Parents.Add(node1);

			try
			{
				var graph = new Graph<TestNode>(nodeCollection);
				var updateOrder = graph.GetUpdateOrder(node => node.Parents);
			}
			catch (Exception e)
			{
				Assert.Equal(typeof(SingularityAggregateException), e.GetType());
				var singularityAggregateException = (SingularityAggregateException)e;
				Assert.Equal(2, singularityAggregateException.InnerExceptions.Count);
				Assert.Equal(typeof(CircularDependencyException), singularityAggregateException.InnerExceptions[0].GetType());
				Assert.Equal(typeof(CircularDependencyException), singularityAggregateException.InnerExceptions[1].GetType());
			}
		}

		[Fact]
		public void CircularDependency_Complex_Throws()
		{
			var nodeCollection = new TestNodeCollection();

			var node1 = nodeCollection.Add("node1");
			var node2 = nodeCollection.Add("node2");
			var node3 = nodeCollection.Add("node3");

			node1.Parents.Add(node3);
			node2.Parents.Add(node1);
			node3.Parents.Add(node2);

			try
			{
				var graph = new Graph<TestNode>(nodeCollection);
				var updateOrder = graph.GetUpdateOrder(node => node.Parents);
			}
			catch (Exception e)
			{
				Assert.Equal(typeof(SingularityAggregateException), e.GetType());
				var singularityAggregateException = (SingularityAggregateException)e;
				Assert.Equal(3, singularityAggregateException.InnerExceptions.Count);
				Assert.Equal(typeof(CircularDependencyException), singularityAggregateException.InnerExceptions[0].GetType());
				Assert.Equal(typeof(CircularDependencyException), singularityAggregateException.InnerExceptions[1].GetType());
				Assert.Equal(typeof(CircularDependencyException), singularityAggregateException.InnerExceptions[2].GetType());
			}
		}
	}
}