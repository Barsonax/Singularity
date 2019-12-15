using System;
using Singularity.Exceptions;
using Singularity.Resolvers;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test
{
    public class BestMatchConstructorResolverTests
    {
        [Fact]
        public void StaticSelectConstructor_SingleConstructor_NoConstructors_Throws()
        {
            //ARRANGE
            Type type = typeof(NoPublicConstructorClass);
            var resolver = new ConstructorResolverCache(new BestMatchConstructorResolver());

            //ACT
            //ASSERT
            Assert.Throws<NoConstructorException>(() =>
            {
                resolver.StaticSelectConstructor(type);
            });

            Assert.Throws<NoConstructorException>(() =>
            {
                resolver.StaticSelectConstructor(type);
            });
        }

        [Fact]
        public void StaticSelectConstructor_MultipleConstructors_ReturnNull()
        {
            //ARRANGE
            Type type = typeof(MultipleConstructorsClass);
            var resolver = new ConstructorResolverCache(new BestMatchConstructorResolver());

            //ACT
            var staticSelectedConstructor = resolver.StaticSelectConstructor(type);

            //ASSERT
            Assert.Null(staticSelectedConstructor);
        }

        [Fact]
        public void DynamicSelectConstructor_MultipleConstructors_NotRegisteredArgument_PicksConstructorWithMostArgumentsThatCanBeResolved()
        {
            //ARRANGE
            Type type = typeof(MultipleConstructorsClass);
            var resolver = new ConstructorResolverCache(new BestMatchConstructorResolver());
            var container = new Container(b =>
            {
                
            });

            //ACT
            var dynamicSelectedConstructor = resolver.DynamicSelectConstructor(type, container.Context);

            //ASSERT
            Assert.NotNull(dynamicSelectedConstructor);
            Assert.Empty(dynamicSelectedConstructor.GetParameters());
        }

        [Fact]
        public void DynamicSelectConstructor_MultipleConstructors_RegisteredArgument_PicksConstructorWithMostArgumentsThatCanBeResolved()
        {
            //ARRANGE
            Type type = typeof(MultipleConstructorsClass);
            var resolver = new ConstructorResolverCache(new BestMatchConstructorResolver());
            var container = new Container(b =>
            {
                b.Register<ITestService10, TestService10>();
            });

            //ACT
            var dynamicSelectedConstructor = resolver.DynamicSelectConstructor(type, container.Context);

            //ASSERT
            Assert.NotNull(dynamicSelectedConstructor);
            var parameter = Assert.Single(dynamicSelectedConstructor.GetParameters());
            Assert.Equal(typeof(ITestService10), parameter.ParameterType);
        }
    }
}