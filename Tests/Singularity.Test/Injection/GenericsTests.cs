using System;
using Singularity.Exceptions;
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
            var container = new Container(builder =>
            {
                builder.Register(typeof(ISerializer<>), typeof(DefaultSerializer<>));
            });

            //ACT
            var serializer = container.GetInstance<ISerializer<Special>>();

            //ASSERT
            Assert.IsType<DefaultSerializer<Special>>(serializer);
        }

        [Fact]
        public void Getinstance_ByOpenGenericType_Throws()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register(typeof(ISerializer<>), typeof(DefaultSerializer<>));
            });

            //ACT
            //ASSERT
            Assert.Throws<AbstractTypeResolveException>(() =>
            {
                object serializer = container.GetInstance(typeof(ISerializer<>));
            });
        }

        [Fact]
        public void NestedOpenGenericType()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register(typeof(ISerializer<>), typeof(DefaultSerializer<>));
                builder.Register(typeof(INestedSerializer<>), typeof(NestedSerializer<>));
            });

            //ACT
            var serializer = container.GetInstance<INestedSerializer<Special>>();

            //ASSERT
            Assert.IsType<NestedSerializer<Special>>(serializer);
        }

        [Fact]
        public void OpenGenericTypeOverride()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register(typeof(ISerializer<>), typeof(DefaultSerializer<>));
                builder.Register<ISerializer<int>, IntSerializer>();
            });

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
            var container = new Container(builder =>
            {
                builder.Register(typeof(ISerializer<>), typeof(DefaultSerializer<>));
                builder.Register(typeof(INestedSerializer<>), typeof(NestedSerializer<>));
                builder.Register<INestedSerializer<int>, NestedIntSerializer>();
            });

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
