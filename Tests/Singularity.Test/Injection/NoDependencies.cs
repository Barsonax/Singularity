using System;
using System.Collections.Generic;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class NoDependencies
    {
        [Fact]
        public void GetInstance_CorrectDependencyIsReturned()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<ITestService10, TestService10>();

            var container = new Container(config);
            Container nestedContainer = container.GetNestedContainer(new BindingConfig());

            //ACT
            var value = container.GetInstance<ITestService10>();
            var nestedValue = nestedContainer.GetInstance<ITestService10>();

            //ASSERT
            Assert.IsType<TestService10>(value);
            Assert.IsType<TestService10>(nestedValue);
        }

        [Fact]
        public void GetInstance_Module_CorrectDependencyIsReturned()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<ITestService10, TestService10>();

            var container = new Container(new[] { new TestModule1() });

            //ACT
            var value = container.GetInstance<ITestService10>();

            //ASSERT
            Assert.IsType<TestService10>(value);
        }

        [Fact]
        public void GetInstance_GetDependencyByConcreteType_ReturnsCorrectDependency()
        {
            //ARRANGE
            var config = new BindingConfig();

            var container = new Container(config);

            //ACT
            var value = container.GetInstance<TestService10>();

            //ASSERT
            Assert.IsType<TestService10>(value);
        }

        [Fact]
        public void GetInstance_ReturnsCorrectDependency()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<ITestService10, TestService10>();

            var container = new Container(config);

            //ACT
            var value = container.GetInstance<ITestService10>();

            //ASSERT
            Assert.IsType<TestService10>(value);
        }

        [Fact]
        public void GetInstance_FuncWithMethodCall_ReturnsCorrectDependency()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<ITestService10>().Inject(() => CreateTestService());

            var container = new Container(config);

            //ACT
            var value = container.GetInstance<ITestService10>();

            //ASSERT
            Assert.IsType<TestService10>(value);
        }

        [Fact]
        public void GetInstance_FuncWithConstructorCall_ReturnsCorrectDependency()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<ITestService10>().Inject(() => new TestService10());

            //ACT
            var container = new Container(config);

            //ASSERT
            var value = container.GetInstance<ITestService10>();
            Assert.IsType<TestService10>(value);
        }

        [Fact]
        public void GetInstance_FuncWithDelegateCall_ReturnsCorrectDependency()
        {
            //ARRANGE
            var config = new BindingConfig();
            Func<TestService10> func = () => new TestService10();
            config.Register<ITestService10>().Inject(() => func.Invoke());

            var container = new Container(config);

            //ACT
            var value = container.GetInstance<ITestService10>();

            //ASSERT
            Assert.IsType<TestService10>(value);
        }

        private TestService10 CreateTestService()
        {
            return new TestService10();
        }

        [Fact]
        public void MethodInject_InjectsCorrectDependencies()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<ITestService10, TestService10>();

            var container = new Container(config);
            var instance = new MethodInjectionClass();

            //ACT
            container.MethodInject(instance);

            //ASSERT
            Assert.IsType<TestService10>(instance.TestService10);
        }

        [Fact]
        public void MethodInjectAll_InjectsCorrectDependencies()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<ITestService10, TestService10>();

            var container = new Container(config);

            var instances = new List<MethodInjectionClass>();
            for (var i = 0; i < 10; i++)
            {
                instances.Add(new MethodInjectionClass());
            }

            //ACT
            container.MethodInjectAll(instances);

            //ASSERT
            foreach (MethodInjectionClass instance in instances)
            {
                Assert.IsType<TestService10>(instance.TestService10);
            }
        }

        [Fact]
        public void GetInstance_PerContainerLifetime_ReturnsSameInstancePerCall()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<ITestService10, TestService10>().With(CreationMode.Singleton);

            var container = new Container(config);

            //ACT
            var value1 = container.GetInstance<ITestService10>();
            var value2 = container.GetInstance<ITestService10>();

            //ASSERT
            Assert.NotNull(value1);
            Assert.NotNull(value2);
            Assert.Same(value1, value2);
        }

        [Fact]
        public void GetInstance_PerCallLifetime_ReturnsNewInstancePerCall()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<ITestService10, TestService10>();

            var container = new Container(config);

            //ACT
            var value1 = container.GetInstance<ITestService10>();
            var value2 = container.GetInstance<ITestService10>();

            //ASSERT
            Assert.NotNull(value1);
            Assert.NotNull(value2);
            Assert.NotSame(value1, value2);
        }
    }
}
