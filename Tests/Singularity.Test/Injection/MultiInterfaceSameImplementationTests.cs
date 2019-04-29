using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class MultiInterfaceSameImplementationTests
    {
        [Fact]
        public void GetInstance_2Interfaces_ReturnsCorrectDependency()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IService1, Implementation1>(c => c
                    .As<IService2>());
            });

            //ACT
            var value1 = container.GetInstance<IService1>();
            var value2 = container.GetInstance<IService2>();
            var value3 = container.GetInstance<Implementation1>();

            //ASSERT
            Assert.IsType<Implementation1>(value1);
            Assert.IsType<Implementation1>(value2);
            Assert.IsType<Implementation1>(value3);

            Assert.NotSame(value1, value2);
            Assert.NotSame(value1, value3);
        }

        [Fact]
        public void GetInstance_3Interfaces_ReturnsCorrectDependency()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IService1, Implementation1>(c => c
                    .As<IService2>()
                    .As<IService3>());
            });

            //ACT
            var value1 = container.GetInstance<IService1>();
            var value2 = container.GetInstance<IService2>();
            var value3 = container.GetInstance<IService3>();
            var value4 = container.GetInstance<Implementation1>();

            //ASSERT
            Assert.IsType<Implementation1>(value1);
            Assert.IsType<Implementation1>(value2);
            Assert.IsType<Implementation1>(value3);
            Assert.IsType<Implementation1>(value4);

            Assert.NotSame(value1, value2);
            Assert.NotSame(value1, value3);
            Assert.NotSame(value1, value4);
        }

        [Fact]
        public void GetInstance_Decorators_ReturnsCorrectDependency()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IService1, Implementation1>(c => c
                    .As<IService2>()
                    .As<IService3>());
                builder.Decorate<IService2, Service2Decorator>();
            });

            //ACT
            var value1 = container.GetInstance<IService1>();
            var serviceDecorator = container.GetInstance<IService2>();
            var value3 = container.GetInstance<IService3>();
            var value4 = container.GetInstance<Implementation1>();

            //ASSERT
            Assert.IsType<Implementation1>(value1);
            var value2 = Assert.IsType<Service2Decorator>(serviceDecorator);
            Assert.IsType<Implementation1>(value3);
            Assert.IsType<Implementation1>(value4);

            Assert.NotSame(value1, value2);
            Assert.NotSame(value1, value3);
            Assert.NotSame(value1, value4);
        }

        [Fact]
        public void GetInstance_WeaklyTyped_ReturnsCorrectDependency()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register(typeof(IService1), typeof(Implementation1), c => c
                    .As(typeof(IService2))
                    .As(typeof(IService3)));
            });

            //ACT
            var value1 = container.GetInstance<IService1>();
            var value2 = container.GetInstance<IService2>();
            var value3 = container.GetInstance<IService3>();
            var value4 = container.GetInstance<Implementation1>();

            //ASSERT
            Assert.IsType<Implementation1>(value1);
            Assert.IsType<Implementation1>(value2);
            Assert.IsType<Implementation1>(value3);
            Assert.IsType<Implementation1>(value4);

            Assert.NotSame(value1, value2);
            Assert.NotSame(value1, value3);
            Assert.NotSame(value1, value4);
        }

        [Fact]
        public void GetInstance_PerContainerLifetime_ReturnsCorrectDependency()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IService1, Implementation1>(c => c
                    .As<IService2>()
                    .As<IService3>()
                    .As<Implementation1>()
                    .With(Lifetime.PerContainer));
            });

            //ACT
            var value1 = container.GetInstance<IService1>();
            var value2 = container.GetInstance<IService2>();
            var value3 = container.GetInstance<IService3>();
            var value4 = container.GetInstance<Implementation1>();

            //ASSERT
            Assert.IsType<Implementation1>(value1);
            Assert.IsType<Implementation1>(value2);
            Assert.IsType<Implementation1>(value3);
            Assert.IsType<Implementation1>(value4);

            Assert.Same(value1, value2);
            Assert.Same(value1, value3);
            Assert.Same(value1, value4);
        }

        [Fact]
        public void GetInstance_WeaklyTyped_PerContainerLifetime_ReturnsCorrectDependency()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register(typeof(IService1), typeof(Implementation1), c => c
                    .As(typeof(IService2))
                    .As(typeof(IService3))
                    .As(typeof(Implementation1))
                    .With(Lifetime.PerContainer));
            });

            //ACT
            var value1 = container.GetInstance<IService1>();
            var value2 = container.GetInstance<IService2>();
            var value3 = container.GetInstance<IService3>();
            var value4 = container.GetInstance<Implementation1>();

            //ASSERT
            Assert.IsType<Implementation1>(value1);
            Assert.IsType<Implementation1>(value2);
            Assert.IsType<Implementation1>(value3);
            Assert.IsType<Implementation1>(value4);

            Assert.Same(value1, value2);
            Assert.Same(value1, value3);
            Assert.Same(value1, value4);
        }
    }
}
