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
            var container = new Container(builder =>
            {
                builder.Register<ITransient1, Transient1>();
                builder.Register<ISingleton1, Singleton1>();
                builder.Register<ICombined1, Combined1>();
            });

            //ACT
            //ASSERT
            Assert.Throws<RegistrationAlreadyExistsException>(() =>
            {
                Container nestedContainer = container.GetNestedContainer(builder =>
                {
                    builder.Register<ITransient1, ScopedTransient>();
                });
            });
        }
    }
}
