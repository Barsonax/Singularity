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
				config.Decorate<TestService10>().With<DecoratorWithNoInterface>();
			});
		}

        [Fact]
        public void Decorate_WrongConstructorArguments_Throws()
        {
            var config = new BindingConfig();
            Assert.Throws<InterfaceNotImplementedException>(() =>
            {
                config.Decorate<ITestService10>().With(typeof(Component));
            });
        }

        [Fact]
        public void Decorate_InvalidConstructorArguments_Throws()
        {
            var config = new BindingConfig();
            Assert.Throws<InvalidExpressionArgumentsException>(() =>
            {
                config.Decorate<ITestService10>().With<DecoratorWrongConstructorArguments>();
            });
        }
    }
}
