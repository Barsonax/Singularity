using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class Decorators
    {
        [Fact]
        public void GetInstance_Decorate_Simple()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Decorate<IComponent, Decorator1>();
                builder.Register<IComponent, Component>();
            });

            //ACT
            var value = container.GetInstance<IComponent>();

            //ASSERT
            var decorator1 = Assert.IsType<Decorator1>(value);
            var component = Assert.IsType<Component>(decorator1.Component);
        }

        [Fact]
        public void GetInstance_Decorate_Complex1()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Decorate<IComponent, Decorator1>();
                builder.Decorate<IComponent, Decorator2>();
                builder.Register<IComponent, Component>();
            });

            //ACT
            var value = container.GetInstance<IComponent>();

            //ASSERT
            var decorator2 = Assert.IsType<Decorator2>(value);
            var decorator1 = Assert.IsType<Decorator1>(decorator2.Component);
            var component = Assert.IsType<Component>(decorator1.Component);
        }

        [Fact]
        public void GetInstance_Decorate_Complex2()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Decorate<ITestService11, TestService11_Decorator1>();
                builder.Register<ITestService10, TestService10>();
                builder.Register<ITestService11, TestService11>();
            });

            //ACT
            var value = container.GetInstance<ITestService11>();

            //ASSERT
            var decorator1 = Assert.IsType<TestService11_Decorator1>(value);
            var testService11 = Assert.IsType<TestService11>(decorator1.TestService11);
            var testService10 = Assert.IsType<TestService10>(testService11.TestService10);
        }

        [Fact]
        public void GetInstance_Decorate_Complex3()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Decorate<ITestService11, TestService11_Decorator1>();
                builder.Decorate<ITestService11, TestService11_Decorator2>();
                builder.Register<ITestService10, TestService10>();
                builder.Register<ITestService11, TestService11>();
            });

            //ACT
            var value = container.GetInstance<ITestService11>();

            //ASSERT
            var decorator2 = Assert.IsType<TestService11_Decorator2>(value);
            var decorator1 = Assert.IsType<TestService11_Decorator1>(decorator2.TestService11);
            var testService11 = Assert.IsType<TestService11>(decorator1.TestService11);
            var testService10 = Assert.IsType<TestService10>(testService11.TestService10);

            Assert.NotSame(decorator2.TestService10, decorator2.TestService10FromIOC);
        }
    }
}
