using System;
using System.Collections.Generic;
using Singularity.Bindings;
using Singularity.Graph;
using Singularity.Test.TestClasses;
using Xunit;

namespace Singularity.Test.Graph
{
    public class DependencyGraphTests
    {
		[Fact]
	    public void Constructor_CorruptLifetimeValue_Throws()
		{
			var config = new BindingConfig();
			config.For<ITestService10>().Inject<TestService10>().With((Lifetime)(-234324524));

			Assert.Throws<ArgumentOutOfRangeException>(() =>
			{
				var dependencyGraph = new DependencyGraph(config, new List<IDependencyExpressionGenerator>());
			});		
		}
    }
}
