using Singularity.Test.TestClasses;
using Xunit;

namespace Singularity.Test
{
    public class ContainerNoInternalDepedendencyTests
    {
        [Fact]
        public void GetInstance_NoInternalDependencies()
        {
            var container = new Container();
            using (var builder = container.StartBuilding())
            {
                builder.Bind<ITestService10>().To<TestService10>();
            }
            var value = container.GetInstance<ITestService10>();
            Assert.Equal(typeof(TestService10), value.GetType());
        }

        [Fact]
        public void GetInstance_WithPerContainerLifetime()
        {
            var container = new Container();
            using (var builder = container.StartBuilding())
            {
                builder.Bind<ITestService10>().To<TestService10>().SetLifetime(Lifetime.PerContainer);
            }
            var value1 = container.GetInstance<ITestService10>();
            var value2 = container.GetInstance<ITestService10>();
            Assert.NotNull(value1);
            Assert.NotNull(value2);
            Assert.Equal(value1, value2);
        }

        [Fact]
        public void GetInstance_WithPerCallLifetime()
        {
            var container = new Container();
            using (var builder = container.StartBuilding())
            {
                builder.Bind<ITestService10>().To<TestService10>();
            }
            var value1 = container.GetInstance<ITestService10>();
            var value2 = container.GetInstance<ITestService10>();
            Assert.NotNull(value1);
            Assert.NotNull(value2);
            Assert.NotEqual(value1, value2);
        }
    }
}