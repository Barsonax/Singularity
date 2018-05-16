using Singularity.Bindings;
using Singularity.Exceptions;
using Singularity.Graph;
using Xunit;

namespace Singularity.Test.TestClasses
{
    public class DependencyGraphTests
    {
        [Fact]
        public void Foo()
        {
            var config = new BindingConfig();
            config.For<ITestService11>().Inject<TestService11>();

            Assert.Throws<CannotResolveDependencyException>(() =>
            {
                var dependencyGraph = new DependencyGraph(config);
            });            
        }
    }
}
