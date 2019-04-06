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
            var config = new BindingConfig();
            config.Decorate<IComponent, Decorator1>();
            config.Register<IComponent, Component>();

            var container = new Container(config);

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
            var config = new BindingConfig();
            config.Decorate<IComponent, Decorator1>();
            config.Decorate<IComponent, Decorator2>();
            config.Register<IComponent, Component>();

            var container = new Container(config);

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
            var config = new BindingConfig();
            config.Decorate<ITestService11, TestService11_Decorator1>();
            config.Register<ITestService10, TestService10>();
            config.Register<ITestService11, TestService11>();

            var container = new Container(config);

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
            var config = new BindingConfig();
            config.Decorate<ITestService11, TestService11_Decorator1>();
            config.Decorate<ITestService11, TestService11_Decorator2>();
            config.Register<ITestService10, TestService10>();
            config.Register<ITestService11, TestService11>();

            var container = new Container(config);

            //ACT
            var value = container.GetInstance<ITestService11>();

            //ASSERT
            var decorator2 = Assert.IsType<TestService11_Decorator2>(value);
            var decorator1 = Assert.IsType<TestService11_Decorator1>(decorator2.TestService11);
            var testService11 = Assert.IsType<TestService11>(decorator1.TestService11);
            var testService10 = Assert.IsType<TestService10>(testService11.TestService10);

            Assert.NotSame(decorator2.TestService10, decorator2.TestService10FromIOC);
        }

        [Fact]
        public void GetInstance_DecoratorInRoot_1Deep_DecoratorsAreCorrectlyApplied()
        {
            //ARRANGE
            var rootConfig = new BindingConfig();
            rootConfig.Decorate<IComponent, Decorator1>();
            var nested1Config = new BindingConfig();
            nested1Config.Register<IComponent, Component>();

            var rootContainer = new Container(rootConfig);
            Container nested1Container = rootContainer.GetNestedContainer(nested1Config);

            //ACT
            var nested1Value = nested1Container.GetInstance<IComponent>();

            //ASSERT
            var nested1Decorator1 = Assert.IsType<Decorator1>(nested1Value);
            var component = Assert.IsType<Component>(nested1Decorator1.Component);
        }

        [Fact]
        public void GetInstance_DecoratorInRoot_2Deep_DecoratorsAreCorrectlyApplied()
        {
            //ARRANGE
            var rootConfig = new BindingConfig();
            rootConfig.Decorate<IComponent, Decorator1>();
            var nested1Config = new BindingConfig();
            nested1Config.Decorate<IComponent, Decorator2>();
            var nested2Config = new BindingConfig();
            nested2Config.Register<IComponent, Component>();

            var rootContainer = new Container(rootConfig);
            Container nested1Container = rootContainer.GetNestedContainer(nested1Config);
            Container nested2Container = nested1Container.GetNestedContainer(nested2Config);

            //ACT
            var nested2Value = nested2Container.GetInstance<IComponent>();

            //ASSERT
            var nested2Decorator2 = Assert.IsType<Decorator2>(nested2Value);
            var nested2Decorator1 = Assert.IsType<Decorator1>(nested2Decorator2.Component);
            var component = Assert.IsType<Component>(nested2Decorator1.Component);
        }

        [Fact]
        public void GetInstance_DecoratorsAreCorrectlyApplied()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Decorate<IComponent, Decorator1>();
            config.Register<IComponent, Component>();
            var nestedConfig = new BindingConfig();
            nestedConfig.Decorate<IComponent, Decorator2>();

            var container = new Container(config);
            Container nestedContainer = container.GetNestedContainer(nestedConfig);

            //ACT
            var value = container.GetInstance<IComponent>();
            var nestedValue = nestedContainer.GetInstance<IComponent>();

            //ASSERT
            var decorator1 = Assert.IsType<Decorator1>(value);
            Assert.IsType<Component>(decorator1.Component);
            var nestedDecorator2 = Assert.IsType<Decorator2>(nestedValue);
            var nestedDecorator1 = Assert.IsType<Decorator1>(nestedDecorator2.Component);
            Assert.IsType<Component>(nestedDecorator1.Component);
        }

        [Fact]
        public void GetInstance_DecorateNestedContainer_Override_PerContainerLifetime()
        {
            //ARRANGE
            var rootConfig = new BindingConfig();
            rootConfig.Decorate<IComponent, Decorator1>();
            rootConfig.Register<IComponent, Component>().With(CreationMode.Singleton);
            var nested1Config = new BindingConfig();
            nested1Config.Decorate<IComponent, Decorator2>();
            nested1Config.Register<IComponent, Component>().With(CreationMode.Singleton);

            var rootContainer = new Container(rootConfig);
            Container nested1Container = rootContainer.GetNestedContainer(nested1Config);

            //ACT
            var rootValue = rootContainer.GetInstance<IComponent>();
            var nested1Value = nested1Container.GetInstance<IComponent>();

            //ASSERT
            var nested1Decorator2 = Assert.IsType<Decorator2>(nested1Value);
            var nested1Decorator1 = Assert.IsType<Decorator1>(nested1Decorator2.Component);
            var component = Assert.IsType<Component>(nested1Decorator1.Component);
        }

        [Fact]
        public void GetInstance_DecorateNestedContainer_PerContainerLifetime()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Decorate<ITestService11, TestService11_Decorator1>();
            config.Register<ITestService10, TestService10>();
            config.Register<ITestService11, TestService11>().With(CreationMode.Singleton);
            var nestedConfig = new BindingConfig();
            nestedConfig.Decorate<ITestService11, TestService11_Decorator2>();

            var container = new Container(config);
            Container nestedContainer = container.GetNestedContainer(nestedConfig);

            //ACT
            var value = container.GetInstance<ITestService11>();

            //ASSERT
            var decorator1 = Assert.IsType<TestService11_Decorator1>(value);
            var testService11 = Assert.IsType<TestService11>(decorator1.TestService11);
            Assert.IsType<TestService10>(testService11.TestService10);
            var nestedInstance = nestedContainer.GetInstance<ITestService11>();
            var nestedDecorator2 = Assert.IsType<TestService11_Decorator2>(nestedInstance);
            var nestedDecorator1 = Assert.IsType<TestService11_Decorator1>(nestedDecorator2.TestService11);
            Assert.IsType<TestService11>(nestedDecorator1.TestService11);
        }

        [Fact]
        public void GetInstance_Decorate_PerContainerLifetime_SameInstanceIsReturned()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IComponent, Component>().With(CreationMode.Singleton);
            var nestedConfig = new BindingConfig();
            nestedConfig.Decorate<IComponent, Decorator1>();

            var container = new Container(config);
            Container nestedContainer = container.GetNestedContainer(nestedConfig);

            //ACT
            var value = container.GetInstance<IComponent>();
            var nestedValue = nestedContainer.GetInstance<IComponent>();

            //ASSERT
            Assert.IsType<Component>(value);
            var decorator1 = Assert.IsType<Decorator1>(nestedValue);
            var component = Assert.IsType<Component>(decorator1.Component);
            Assert.Equal(value, component);
        }
    }
}
