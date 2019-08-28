using System;
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
            var container = new Container(builder =>
            {
                builder.Register<ITestService10, TestService10>();
            });
            Container nestedContainer = container.GetNestedContainer();

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
            var container = new Container();

            //ACT
            var value = container.GetInstance<TestService10>();

            //ASSERT
            Assert.IsType<TestService10>(value);
        }

        [Fact]
        public void GetInstance_ReturnsCorrectDependency()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ITestService10, TestService10>();
            });

            //ACT
            var value = container.GetInstance<ITestService10>();

            //ASSERT
            Assert.IsType<TestService10>(value);
        }

        [Fact]
        public void GetInstance_FuncWithMethodCall_ReturnsCorrectDependency()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ITestService10>(c => c.Inject(() => CreateTestService()));
            });

            //ACT
            var value = container.GetInstance<ITestService10>();

            //ASSERT
            Assert.IsType<TestService10>(value);
        }

        [Fact]
        public void GetInstance_FuncWithConstructorCall_ReturnsCorrectDependency()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ITestService10>(c => c.Inject(() => new TestService10()));
            });

            //ACT
            var value = container.GetInstance<ITestService10>();

            //ASSERT
            Assert.IsType<TestService10>(value);
        }

        [Fact]
        public void GetInstance_FuncWithDelegateCall_ReturnsCorrectDependency()
        {
            //ARRANGE
            Func<TestService10> func = () => new TestService10();
            var container = new Container(builder =>
            {
                builder.Register<ITestService10>(c => c.Inject(() => func.Invoke()));
            });

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
        public void GetInstance_PerContainerLifetime_ReturnsSameInstancePerCall()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ITestService10, TestService10>(c => c.With(Lifetimes.PerContainer));
            });

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
            var container = new Container(builder =>
            {
                builder.Register<ITestService10, TestService10>();
            });

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
