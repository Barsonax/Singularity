using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{

    public class GenericsTests
    {
        [Fact]
        public void OpenGenericType()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register(typeof(ISerializer<>), typeof(DefaultSerializer<>));
            var container = new Container(config);

            //ACT
            var serializer = container.GetInstance<ISerializer<Special>>();

            //ASSERT
            Assert.IsType<DefaultSerializer<Special>>(serializer);
        }

        [Fact]
        public void OpenGenericTypeNestedContainer()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register(typeof(ISerializer<>), typeof(DefaultSerializer<>));
            var nestedConfig = new BindingConfig();
            nestedConfig.Register<ISerializer<int>, IntSerializer>();
            var container = new Container(config);

            //ACT
            var serializer = container.GetInstance<ISerializer<int>>();
            Container nestedContainer = container.GetNestedContainer(nestedConfig);
            var intSerializer = nestedContainer.GetInstance<ISerializer<int>>();

            //ASSERT
            Assert.IsType<DefaultSerializer<int>>(serializer);
            Assert.IsType<IntSerializer>(intSerializer);
        }

        [Fact]
        public void NestedOpenGenericType()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register(typeof(ISerializer<>), typeof(DefaultSerializer<>));
            config.Register(typeof(INestedSerializer<>), typeof(NestedSerializer<>));
            var container = new Container(config);

            //ACT
            var serializer = container.GetInstance<INestedSerializer<Special>>();

            //ASSERT
            Assert.IsType<NestedSerializer<Special>>(serializer);
        }

        [Fact]
        public void OpenGenericTypeOverride()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register(typeof(ISerializer<>), typeof(DefaultSerializer<>));
            config.Register<ISerializer<int>, IntSerializer>();
            var container = new Container(config);

            //ACT
            var intSerializer = container.GetInstance<ISerializer<int>>();
            var serializer = container.GetInstance<ISerializer<Special>>();

            //ASSERT
            Assert.IsType<IntSerializer>(intSerializer);
            Assert.IsType<DefaultSerializer<Special>>(serializer);
        }

        [Fact]
        public void NestedOpenGenericTypeOverride()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register(typeof(ISerializer<>), typeof(DefaultSerializer<>));
            config.Register(typeof(INestedSerializer<>), typeof(NestedSerializer<>));
            config.Register<INestedSerializer<int>, NestedIntSerializer>();
            var container = new Container(config);

            //ACT
            var nestedSerializer = container.GetInstance<INestedSerializer<Special>>();
            var floatSerializer = container.GetInstance<ISerializer<Special>>();
            var nestedIntSerializer = container.GetInstance<INestedSerializer<int>>();

            //ASSERT
            Assert.IsType<NestedSerializer<Special>>(nestedSerializer);
            Assert.IsType<DefaultSerializer<Special>>(floatSerializer);
            Assert.IsType<NestedIntSerializer>(nestedIntSerializer);
        }
    }
}
