using System.Collections.Generic;
using Singularity.Enums;
using Singularity.Exceptions;
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
			config.Register<ITestService10, TestService10>().With((Lifetime)(-234324524));

			var aggregateException = Assert.Throws<SingularityAggregateException>(() =>
			{
				var dependencyGraph = new DependencyGraph(config, new List<IDependencyExpressionGenerator>());
			});
			var innerExceptions = aggregateException.Flatten().InnerExceptions;
			var exception = Assert.Single(innerExceptions);
			Assert.IsType<InvalidLifetimeException>(exception);
		}
    }
}
