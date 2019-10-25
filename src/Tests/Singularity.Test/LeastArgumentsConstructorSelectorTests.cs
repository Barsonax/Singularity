using System;
using Singularity.Exceptions;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test
{
    public class LeastArgumentsConstructorSelectorTests
    {
        [Fact]
        public void SelectConstructor_SingleConstructor_NoConstructors_Throws()
        {
            Type type = typeof(NoPublicConstructorClass);
            Assert.Throws<NoConstructorException>(() =>
            {
                ConstructorResolvers.LeastArguments.SelectConstructor(type);
            });
        }

        [Fact]
        public void SelectConstructor_MultipleConstructors()
        {
            Type type = typeof(MultipleConstructorsClass);

            var selectedConstructor = ConstructorResolvers.LeastArguments.SelectConstructor(type);

            Assert.Empty(selectedConstructor.GetParameters());
        }
    }
}