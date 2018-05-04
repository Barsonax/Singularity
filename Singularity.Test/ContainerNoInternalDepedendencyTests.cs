using Singularity.Test.TestClasses;
using Xunit;

namespace Singularity.Test
{
    public class ContainerNoInternalDepedendencyTests
    {
        [Fact]
        public void GetInstance_NoInternalDependencies()
        {
            var config = new BindingConfig();
            config.Bind<ITestService10>().To<TestService10>();

            var container = new Container(config);

            var value = container.GetInstance<ITestService10>();
            Assert.Equal(typeof(TestService10), value.GetType());
        }

        [Fact]
        public void GetInstance_WithPerContainerLifetime()
        {
            var config = new BindingConfig();
            config.Bind<ITestService10>().To<TestService10>().SetLifetime(Lifetime.PerContainer);

            var container = new Container(config);       

            var value1 = container.GetInstance<ITestService10>();
            var value2 = container.GetInstance<ITestService10>();
            Assert.NotNull(value1);
            Assert.NotNull(value2);
            Assert.Equal(value1, value2);
        }

        [Fact]
        public void GetInstance_WithPerCallLifetime()
        {
            var config = new BindingConfig();
            config.Bind<ITestService10>().To<TestService10>();

            var container = new Container(config);

            var value1 = container.GetInstance<ITestService10>();
            var value2 = container.GetInstance<ITestService10>();
            Assert.NotNull(value1);
            Assert.NotNull(value2);
            Assert.NotEqual(value1, value2);
        }
    }
}