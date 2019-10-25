using System;
using Singularity.Exceptions;
using Singularity.Expressions;
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
            Assert.Throws<NoConstructorException>(() =>
            {
                ConstructorResolvers.Default.SelectConstructor(type);
            });
        }

        [Fact]
        public void SelectConstructor_MultipleConstructors_Throws()
        {
            Type type = typeof(MultipleConstructorsClass);
            Assert.Throws<CannotAutoResolveConstructorException>(() =>
            {
                ConstructorResolvers.Default.SelectConstructor(type);
            });
        }
    }
}