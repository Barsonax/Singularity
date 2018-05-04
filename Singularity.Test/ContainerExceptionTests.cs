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
            
            Assert.Throws<CannotResolveDependencyException>(() =>
            {
                var config = new BindingConfig();
                config.Bind<ITestService11>().To<TestService11>();
                var container = new Container(config);
            });
        }
    }
}
