using System;
using Singularity.Exceptions;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test
{
    public class DefaultConstructorSelectorTests
    {
        [Fact]
        public void SelectConstructor_SingleConstructor_NoConstructors_Throws()
        {
            Type type = typeof(NoPublicConstructorClass);
            var resolver = new ConstructorResolverCache(new DefaultConstructorResolver());
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
        public void SelectConstructor_MultipleConstructors_Throws()
        {
            Type type = typeof(MultipleConstructorsClass);
            var resolver = new ConstructorResolverCache(new DefaultConstructorResolver());

            Assert.Throws<CannotAutoResolveConstructorException>(() =>
            {
                resolver.StaticSelectConstructor(type);
            });

            Assert.Throws<CannotAutoResolveConstructorException>(() =>
            {
                resolver.StaticSelectConstructor(type);
            });
        }
    }
}