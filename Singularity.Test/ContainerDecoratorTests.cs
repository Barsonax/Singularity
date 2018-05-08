using Singularity.Test.TestClasses;
using Xunit;

namespace Singularity.Test
{
    public class ContainerDecoratorTests
    {
        [Fact]
        public void GetInstance_Decorate_Simple()
        {
            var config = new BindingConfig();
            config.Decorate<Decorator1>().On<IComponent>();
            config.Bind<IComponent>().To<Component>();

            var container = new Container(config);

            var value = container.GetInstance<IComponent>();

            Assert.NotNull(value);
            Assert.Equal(typeof(Decorator1), value.GetType());
            var decorator1 = (Decorator1)value;
            Assert.Equal(typeof(Component), decorator1.Component.GetType());
        }

        [Fact]
        public void GetInstance_Decorate_Complex1()
        {
            var config = new BindingConfig();

            config.Decorate<Decorator1>().On<IComponent>();
            config.Decorate<Decorator2>().On<IComponent>();

            config.Bind<IComponent>().To<Component>();

            var container = new Container(config);

            var value = container.GetInstance<IComponent>();

            Assert.NotNull(value);
            Assert.Equal(typeof(Decorator2), value.GetType());
            var decorator2 = (Decorator2)value;

            Assert.Equal(typeof(Decorator1), decorator2.Component.GetType());
            var decorator1 = (Decorator1)decorator2.Component;

            Assert.Equal(typeof(Component), decorator1.Component.GetType());
        }

        [Fact]
        public void GetInstance_Decorate_Complex2()
        {
            var config = new BindingConfig();

            config.Decorate<TestService11_Decorator1>().On<ITestService11>();

            config.Bind<ITestService10>().To<TestService10>();
            config.Bind<ITestService11>().To<TestService11>();

            var container = new Container(config);

            var value = container.GetInstance<ITestService11>();

            Assert.NotNull(value);
            Assert.Equal(typeof(TestService11_Decorator1), value.GetType());
            var decorator1 = (TestService11_Decorator1)value;

            Assert.NotNull(decorator1.TestService11);
            Assert.Equal(typeof(TestService11), decorator1.TestService11.GetType());
            var testService11 = (TestService11)decorator1.TestService11;

            Assert.NotNull(testService11.TestService10);
            Assert.Equal(typeof(TestService10), testService11.TestService10.GetType());
        }

        [Fact]
        public void GetInstance_Decorate_Complex3()
        {
            var config = new BindingConfig();

            config.Decorate<TestService11_Decorator1>().On<ITestService11>();
            config.Decorate<TestService11_Decorator2>().On<ITestService11>();

            config.Bind<ITestService10>().To<TestService10>();
            config.Bind<ITestService11>().To<TestService11>();

            var container = new Container(config);

            var value = container.GetInstance<ITestService11>();

            Assert.NotNull(value);
            Assert.Equal(typeof(TestService11_Decorator2), value.GetType());
            var decorator2 = (TestService11_Decorator2)value;

            Assert.NotNull(decorator2.TestService11);
            Assert.NotEqual(decorator2.TestService10, decorator2.TestService10FromIOC);
            Assert.Equal(typeof(TestService11_Decorator1), decorator2.TestService11.GetType());
            var decorator1 = (TestService11_Decorator1)decorator2.TestService11;

            Assert.NotNull(decorator1.TestService11);
            Assert.Equal(typeof(TestService11), decorator1.TestService11.GetType());
            var testService11 = (TestService11)decorator1.TestService11;

            Assert.NotNull(testService11.TestService10);
            Assert.Equal(typeof(TestService10), testService11.TestService10.GetType());
        }
    }
}
