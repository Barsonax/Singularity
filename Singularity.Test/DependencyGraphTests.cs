using System;
using System.Collections.Generic;
using System.Text;
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

			var graph = new DependencyGraph(config.Bindings.Values);
			Assert.Equal(3, graph.Nodes.Count);
			Assert.Equal(3, graph.UpdateOrder.Count);

			Assert.Equal(1, graph.UpdateOrder[0].Count);
			Assert.Equal(typeof(TestService10), graph.UpdateOrder[0][0].ActualType);

			Assert.Equal(1, graph.UpdateOrder[1].Count);
			Assert.Equal(typeof(TestService11), graph.UpdateOrder[1][0].ActualType);

			Assert.Equal(1, graph.UpdateOrder[2].Count);
			Assert.Equal(typeof(TestService12), graph.UpdateOrder[2][0].ActualType);
		}

		[Fact]
		public void CheckUpdateOrder_Simple2()
		{
			var builder = new BindingConfig();

			builder.Bind<ICompositeTestService13>().To<CompositeTestService13>();
			builder.Bind<ITestService10>().To<TestService10>();
			builder.Bind<ITestService11>().To<TestService11>();
			builder.Bind<ITestService12>().To<TestService12>();

			builder.Bind<ITestService20>().To<TestService20>();
			builder.Bind<ITestService21>().To<TestService21>();
			builder.Bind<ITestService22>().To<TestService22>();

			builder.Bind<ITestService30>().To<TestService30>();
			builder.Bind<ITestService31>().To<TestService31>();
			builder.Bind<ITestService32>().To<TestService32>();

			var graph = new DependencyGraph(builder.Bindings.Values);
		}
	}
}
