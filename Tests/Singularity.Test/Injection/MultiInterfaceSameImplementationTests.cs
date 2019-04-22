using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class MultiInterfaceSameImplementationTests
    {
        [Fact]
        public void GetInstance_MultiInterfacesSameImplementation_ReturnsCorrectDependency()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IService1, IService2, IService3, Implementation1>();

            var container = new Container(config);

            //ACT
            var value1 = container.GetInstance<IService1>();
            var value2 = container.GetInstance<IService2>();
            var value3 = container.GetInstance<IService3>();

            //ASSERT
            Assert.IsType<Implementation1>(value1);
            Assert.IsType<Implementation1>(value2);
            Assert.IsType<Implementation1>(value3);

            Assert.NotSame(value1, value2);
            Assert.NotSame(value1, value3);
        }

        [Fact]
        public void GetInstance_MultiInterfacesSameImplementation_WeaklyTyped_ReturnsCorrectDependency()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register(new[] { typeof(IService1), typeof(IService2), typeof(IService3) }, typeof(Implementation1));

            var container = new Container(config);

            //ACT
            var value1 = container.GetInstance<IService1>();
            var value2 = container.GetInstance<IService2>();
            var value3 = container.GetInstance<IService3>();

            //ASSERT
            Assert.IsType<Implementation1>(value1);
            Assert.IsType<Implementation1>(value2);
            Assert.IsType<Implementation1>(value3);

            Assert.NotSame(value1, value2);
            Assert.NotSame(value1, value3);
        }

        [Fact]
        public void GetInstance_MultiInterfacesSameImplementation_PerContainerLifetime_ReturnsCorrectDependency()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IService1, IService2, IService3, Implementation1>().With(Lifetime.PerContainer);

            var container = new Container(config);

            //ACT
            var value1 = container.GetInstance<IService1>();
            var value2 = container.GetInstance<IService2>();
            var value3 = container.GetInstance<IService3>();

            //ASSERT
            Assert.IsType<Implementation1>(value1);
            Assert.IsType<Implementation1>(value2);
            Assert.IsType<Implementation1>(value3);

            Assert.Same(value1, value2);
            Assert.Same(value1, value3);
        }

        [Fact]
        public void GetInstance_MultiInterfacesSameImplementation_WeaklyTyped_PerContainerLifetime_ReturnsCorrectDependency()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register(new[] { typeof(IService1), typeof(IService2), typeof(IService3) }, typeof(Implementation1)).With(Lifetime.PerContainer);

            var container = new Container(config);

            //ACT
            var value1 = container.GetInstance<IService1>();
            var value2 = container.GetInstance<IService2>();
            var value3 = container.GetInstance<IService3>();

            //ASSERT
            Assert.IsType<Implementation1>(value1);
            Assert.IsType<Implementation1>(value2);
            Assert.IsType<Implementation1>(value3);

            Assert.Same(value1, value2);
            Assert.Same(value1, value3);
        }
    }
}
