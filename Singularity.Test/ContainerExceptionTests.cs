using System;
using Singularity.Bindings;
using Xunit;
using Singularity.Exceptions;
using Singularity.Test.TestClasses;

namespace Singularity.Test
{
	public class ContainerExceptionTests
	{
		[Fact]
		public void GetInstance_MissingDependency_Throws()
		{
			var container = new Container(new BindingConfig());
			Assert.Throws<DependencyNotFoundException>(() =>
			{
				container.GetInstance<ITestService10>();
			});
		}

		[Fact]
		public void GetInstance_MissingInternalDependency_Throws()
		{
			try
			{
				var config = new BindingConfig();
				config.For<ITestService11>().Inject<TestService11>();
				var container = new Container(config);
			}
			catch (AggregateException e)
			{
			    Assert.Equal(typeof(SingularityAggregateException), e.GetType());
				var aggregateException = e.Flatten();

				Assert.Equal(1, aggregateException.InnerExceptions.Count);
				Assert.Equal(typeof(DependencyNotFoundException), aggregateException.InnerExceptions[0].GetType());
				var dependencyNotFoundException = (DependencyNotFoundException)aggregateException.InnerExceptions[0];

				Assert.Equal(typeof(ITestService10), dependencyNotFoundException.Type);
			}
		}
	}
}
