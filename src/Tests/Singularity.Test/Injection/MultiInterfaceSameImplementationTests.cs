using System.Collections.Generic;
using System.Linq;
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
        public void GetInstance_Decorators_Enumerable_ReturnsCorrectDependency()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IService1, Implementation2>();
                builder.Register<IService1, Implementation1>(c => c
                    .As<IService2>()
                    .As<IService3>());
                builder.Decorate<IService1, Service1Decorator>();
                builder.Decorate<IService2, Service2Decorator>();
            });

            //ACT
            IService1[] service1s = container.GetInstance<IEnumerable<IService1>>().ToArray();
            IService2[] service2s = container.GetInstance<IEnumerable<IService2>>().ToArray();
            IService3[] service3s = container.GetInstance<IEnumerable<IService3>>().ToArray();

            //ASSERT
            Assert.Equal(2, service1s.Length);
            IService2 service2 = Assert.Single(service2s);
            IService3 service3 = Assert.Single(service3s);

            var service1Decorator1 = Assert.IsType<Service1Decorator>(service1s[0]);
            var service1Decorator2 = Assert.IsType<Service1Decorator>(service1s[1]);
            Assert.NotSame(service1Decorator1, service1Decorator2);

            Assert.IsType<Implementation2>(service1Decorator1.Service);
            var implementation1_1 = Assert.IsType<Implementation1>(service1Decorator2.Service);

            var service2Decorator = Assert.IsType<Service2Decorator>(service2);
            var implementation1_2 = Assert.IsType<Implementation1>(service2Decorator.Service);
            var implementation1_3 = Assert.IsType<Implementation1>(service3);

            Assert.NotSame(implementation1_1, implementation1_2);
            Assert.NotSame(implementation1_1, implementation1_3);
        }

        [Fact]
        public void GetInstance_Decorators_Enumerable_PerContainer_ReturnsCorrectDependency()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IService1, Implementation2>();
                builder.Register<IService1, Implementation1>(c => c
                    .As<IService2>()
                    .As<IService3>()
                    .With(Lifetimes.PerContainer));
                builder.Decorate<IService1, Service1Decorator>();
                builder.Decorate<IService2, Service2Decorator>();
            });

            //ACT
            IService1[] service1s = container.GetInstance<IEnumerable<IService1>>().ToArray();
            IService2[] service2s = container.GetInstance<IEnumerable<IService2>>().ToArray();
            IService3[] service3s = container.GetInstance<IEnumerable<IService3>>().ToArray();

            //ASSERT
            Assert.Equal(2, service1s.Length);
            IService2 service2 = Assert.Single(service2s);
            IService3 service3 = Assert.Single(service3s);

            var service1Decorator1 = Assert.IsType<Service1Decorator>(service1s[0]);
            var service1Decorator2 = Assert.IsType<Service1Decorator>(service1s[1]);
            Assert.NotSame(service1Decorator1, service1Decorator2);

            Assert.IsType<Implementation2>(service1Decorator1.Service);
            var implementation1_1 = Assert.IsType<Implementation1>(service1Decorator2.Service);

            var service2Decorator = Assert.IsType<Service2Decorator>(service2);
            var implementation1_2 = Assert.IsType<Implementation1>(service2Decorator.Service);
            var implementation1_3 = Assert.IsType<Implementation1>(service3);

            Assert.Same(implementation1_1, implementation1_2);
            Assert.Same(implementation1_1, implementation1_3);
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
                    .With(Lifetimes.PerContainer));
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
                    .With(Lifetimes.PerContainer));
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
