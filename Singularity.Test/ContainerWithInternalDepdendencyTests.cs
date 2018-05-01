using Singularity.Test.TestClasses;
using Xunit;

namespace Singularity.Test
{
    public class ContainerWithInternalDepdendencyTests
    {
        [Fact]
        public void GetInstance_1Deep_PerCallLifetime()
        {
            var container = new Container();
            using (var builder = container.StartBuilding())
            {
                builder.Bind<ITestService10>().To<TestService10>();
                builder.Bind<ITestService11>().To<TestService11>();
            }
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
            var container = new Container();
            using (var builder = container.StartBuilding())
            {
                builder.Bind<ITestService10>().To<TestService10>().SetLifetime(Lifetime.PerContainer);
                builder.Bind<ITestService11>().To<TestService11>();
            }
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
            var container = new Container();
            using (var builder = container.StartBuilding())
            {
                builder.Bind<ITestService10>().To<TestService10>();
                builder.Bind<ITestService11>().To<TestService11>();
                builder.Bind<ITestService12>().To<TestService12>();
            }
            var value = container.GetInstance<ITestService12>();
            Assert.Equal(typeof(TestService12), value.GetType());
            Assert.NotNull(value.TestService11);
            Assert.NotNull(value.TestService11.TestService10);
        }
    }
}