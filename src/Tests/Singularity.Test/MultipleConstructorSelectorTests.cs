using System;
using Singularity.Exceptions;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test
{
    public class MultipleConstructorSelectorTests
    {
        [Fact]
        public void SelectConstructor_SingleConstructor_NoConstructors_Throws()
        {
            Type type = typeof(NoPublicConstructorClass);
            Assert.Throws<NoConstructorException>(() =>
            {
                ConstructorSelectors.Multiple.SelectConstructor(type);
            });
        }

        [Fact]
        public void SelectConstructor_MultipleConstructors_Throws()
        {
            Type type = typeof(MultipleConstructorsClass);

            var selectedConstructor = ConstructorSelectors.Multiple.SelectConstructor(type);

            var constructorParameter = Assert.Single(selectedConstructor.GetParameters());
            Assert.Equal(typeof(int), constructorParameter.ParameterType);
        }
    }
}