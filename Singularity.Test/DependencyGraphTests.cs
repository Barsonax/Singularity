using System.Linq;
using System.Net;
using Singularity.Extensions;
using Singularity.Graph;
using Singularity.Test.TestClasses;
using Xunit;

namespace Singularity.Test
{
	public class DependencyGraphTests
	{
		[Fact]
		public void CheckUpdateOrder_Simple()
		{
			var config = new BindingConfig();

			config.Bind<ITestService10>().To<TestService10>();
			config.Bind<ITestService11>().To<TestService11>();
			config.Bind<ITestService12>().To<TestService12>();

			var dependencyGraph = new DependencyGraph(config);

		    var graph = new Graph<DependencyNode>(dependencyGraph.Dependencies.Values);
		    var updateOrder = graph.GetUpdateOrder(node => node.Expression.GetParameterExpressions().Select(x => dependencyGraph.GetDependency(x.Type)));

			Assert.Equal(3, dependencyGraph.Dependencies.Count);
			Assert.Equal(3, updateOrder.Length);

			Assert.Equal(1, updateOrder[0].Length);
			Assert.Equal(typeof(TestService10), updateOrder[0][0].Expression.Type);

			Assert.Equal(1, updateOrder[1].Length);
			Assert.Equal(typeof(TestService11), updateOrder[1][0].Expression.Type);

			Assert.Equal(1, updateOrder[2].Length);
			Assert.Equal(typeof(TestService12), updateOrder[2][0].Expression.Type);
		}

	    [Fact]
	    public void CheckUpdateOrder_Complex()
	    {
	        var config = new BindingConfig();

	        config.Bind<ICompositeTestService13>().To<CompositeTestService13>();
	        config.Bind<ITestService10>().To<TestService10>();
	        config.Bind<ITestService11>().To<TestService11>();
	        config.Bind<ITestService12>().To<TestService12>();

	        var dependencyGraph = new DependencyGraph(config);

	        var graph = new Graph<DependencyNode>(dependencyGraph.Dependencies.Values);
	        var updateOrder = graph.GetUpdateOrder(node => node.Expression.GetParameterExpressions().Select(x => dependencyGraph.GetDependency(x.Type)));

            Assert.Equal(4, dependencyGraph.Dependencies.Count);
	        Assert.Equal(4, updateOrder.Length);

	        Assert.Equal(1, updateOrder[0].Length);
	        Assert.Equal(typeof(TestService10), updateOrder[0][0].Expression.Type);

	        Assert.Equal(1, updateOrder[1].Length);
	        Assert.Equal(typeof(TestService11), updateOrder[1][0].Expression.Type);

	        Assert.Equal(1, updateOrder[2].Length);
	        Assert.Equal(typeof(TestService12), updateOrder[2][0].Expression.Type);

	        Assert.Equal(1, updateOrder[3].Length);
	        Assert.Equal(typeof(CompositeTestService13), updateOrder[3][0].Expression.Type);
        }

        [Fact]
		public void CircularDependency_Simple_Throws()
		{
			var config = new BindingConfig();

			config.Bind<ICircularDependency1>().To<CircularDependency1>();
			config.Bind<ICircularDependency2>().To<CircularDependency2>();

			Assert.Throws<CircularDependencyException>(() =>
			{
				var graph = new DependencyGraph(config);
			});			
		}

	    [Fact]
	    public void CircularDependency_Complex_Throws()
	    {
	        var config = new BindingConfig();

	        config.Bind<ICircularDependencyComplex1>().To<CircularDependencyComplex1>();
	        config.Bind<ICircularDependencyComplex2>().To<CircularDependencyComplex2>();
	        config.Bind<ICircularDependencyComplex3>().To<CircularDependencyComplex3>();

            Assert.Throws<CircularDependencyException>(() =>
	        {
	            var graph = new DependencyGraph(config);
	        });
	    }
	}
}
