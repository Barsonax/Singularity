using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class ChildContainerTests
    {
        [Fact]
        public void OverrideRecursiveRegistration()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<ITransient1, Transient1>();
            config.Register<ISingleton1, Singleton1>();
            config.Register<ICombined1, Combined1>();

            var nestedConfig = new BindingConfig();
            nestedConfig.Register<ITransient1, ScopedTransient>();

            var container = new Container(config);
            Container nestedContainer = container.GetNestedContainer(nestedConfig);

            //ACT
            var combined = container.GetInstance<ICombined1>();
            var scopedCombined = nestedContainer.GetInstance<ICombined1>();

            //ASSERT
            Assert.IsType<Transient1>(combined.Transient);
            Assert.IsType<ScopedTransient>(scopedCombined.Transient);
        }
    }
}
