using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class MultipleImplementationSameInterfaceTests
    {
        [Fact]
        public void Foo()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IService1, IService2, Implementation2>();
                builder.Register<IService1, Implementation1>();
            });

            //ACT
            var service1 = container.GetInstance<IService1>();

            //ASSERT
            Assert.IsType<Implementation1>(service1);
        }

        [Fact]
        public void Foo2()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IService1, IService2, Implementation1>();
                builder.Register<IService1, Implementation2>();
            });

            //ACT
            var service1 = container.GetInstance<IService1>();

            //ASSERT
            Assert.IsType<Implementation2>(service1);
        }

        [Fact]
        public void Foo3()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IService1, Implementation1>();
                builder.Register<IService1, Implementation2>();
            });

            //ACT
            var service1 = container.GetInstance<IService1>();

            //ASSERT
            Assert.IsType<Implementation2>(service1);
        }
    }
}
