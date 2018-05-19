using System;
using Singularity.Bindings;
using Xunit;
using Singularity.Exceptions;
using Singularity.Graph;
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
			catch (Exception e)
			{
				var graphException = (GraphAggregrateException)e;

				Assert.Equal(1, graphException.InnerExceptions.Length);
				Assert.Equal(typeof(CannotResolveDependenciesException), graphException.InnerExceptions[0].GetType());
				var cannotResolveDependenciesException = (CannotResolveDependenciesException)graphException.InnerExceptions[0];

				Assert.Equal(1, cannotResolveDependenciesException.UnresolvedTypes.Length);
				Assert.Equal(typeof(ITestService10), cannotResolveDependenciesException.UnresolvedTypes[0]);
			}
		}
	}
}
