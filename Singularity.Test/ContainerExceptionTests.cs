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
            var container = new Container();
            Assert.Throws<DependencyNotFoundException>(() =>
            {
                container.GetInstance<ITestService10>();
            });
        }

        [Fact]
        public void GetInstance_MissingInternalDependency_Throws()
        {
            var container = new Container();
            Assert.Throws<CannotResolveDependencyException>(() =>
            {
                using (var builder = container.StartBuilding())
                {
                    builder.Bind<ITestService11>().To<TestService11>();
                }
            });
        }
    }
}
