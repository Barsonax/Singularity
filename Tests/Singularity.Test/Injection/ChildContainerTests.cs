using Singularity.Exceptions;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class ChildContainerTests
    {
        [Fact]
        public void GetNestedContainer_OverrideInChild_Throws()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<ITransient1, Transient1>();
            config.Register<ISingleton1, Singleton1>();
            config.Register<ICombined1, Combined1>();

            var nestedConfig = new BindingConfig();
            nestedConfig.Register<ITransient1, ScopedTransient>();

            var container = new Container(config);

            //ACT
            //ASSERT
            Assert.Throws<RegistrationAlreadyExistsException>(() =>
            {
                Container nestedContainer = container.GetNestedContainer(nestedConfig);
            });
        }
    }
}
