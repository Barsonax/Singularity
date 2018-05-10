using Singularity.Test.TestClasses;
using Xunit;

namespace Singularity.Test
{
    public class ContainerWithInternalDepdendencyTests
    {
        [Fact]
        public void GetInstance_1Deep_PerCallLifetime()
        {
            var config = new BindingConfig();
            config.For<ITestService10>().Inject<TestService10>();
            config.For<ITestService11>().Inject<TestService11>();

            var container = new Container(config);

            var value1 = container.GetInstance<ITestService11>();
            var value2 = container.GetInstance<ITestService11>();
            Assert.NotNull(value1);
            Assert.NotNull(value2);
            Assert.NotNull(value1.TestService10);
            Assert.NotNull(value2.TestService10);
            Assert.Equal(typeof(TestService11), value1.GetType());
            Assert.Equal(typeof(TestService11), value2.GetType());
            Assert.NotEqual(value1.TestService10, value2.TestService10);
        }

        [Fact]
        public void GetInstance_1Deep_PerContainerLifetime()
        {
            var config = new BindingConfig();
            config.For<ITestService10>().Inject<TestService10>().With(Lifetime.PerContainer);
            config.For<ITestService11>().Inject<TestService11>();

            var container = new Container(config);

            var value1 = container.GetInstance<ITestService11>();
            var value2 = container.GetInstance<ITestService11>();
            Assert.NotNull(value1);
            Assert.NotNull(value2);
            Assert.NotNull(value1.TestService10);
            Assert.NotNull(value2.TestService10);
            Assert.Equal(typeof(TestService11), value1.GetType());
            Assert.Equal(typeof(TestService11), value2.GetType());
            Assert.Equal(value1.TestService10, value2.TestService10);
        }

        [Fact]
        public void GetInstance_2Deep()
        {
            var config = new BindingConfig();
            config.For<ITestService10>().Inject<TestService10>();
            config.For<ITestService11>().Inject<TestService11>();
            config.For<ITestService12>().Inject<TestService12>();

            var container = new Container(config);

            var value = container.GetInstance<ITestService12>();
            Assert.Equal(typeof(TestService12), value.GetType());
            Assert.NotNull(value.TestService11);
            Assert.NotNull(value.TestService11.TestService10);
        }
    }
}