using Singularity.Test.TestClasses;
using Xunit;

namespace Singularity.Test
{
    public class BindingConfigTests
    {
        [Fact]
        public void Decorate_NotAInterface_Throws()
        {
            var config = new BindingConfig();
            Assert.Throws<InterfaceExpectedException>(() =>
            {
                config.Decorate<Decorator1>().On<Component>();
            });         
        }

        [Fact]
        public void Decorate_InterfaceNotImplemented_Throws()
        {
            var config = new BindingConfig();
            Assert.Throws<InterfaceNotImplementedException>(() =>
            {
                config.Decorate<Decorator1>().On<ITestService10>();
            });
        }

        [Fact]
        public void Decorate_InvalidConstructorArguments_Throws()
        {
            var config = new BindingConfig();
            Assert.Throws<InvalidExpressionArgumentsException>(() =>
            {
                config.Decorate<DecoratorWrongInterface>().On<ITestService10>();
            });
        }
    }
}
