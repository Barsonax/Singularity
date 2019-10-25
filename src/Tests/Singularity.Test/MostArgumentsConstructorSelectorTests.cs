using System;
using Singularity.Exceptions;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test
{
    public class MostArgumentsConstructorSelectorTests
    {
        [Fact]
        public void SelectConstructor_SingleConstructor_NoConstructors_Throws()
        {
            Type type = typeof(NoPublicConstructorClass);
            Assert.Throws<NoConstructorException>(() =>
            {
                ConstructorResolvers.MostArguments.SelectConstructor(type);
            });
        }

        [Fact]
        public void SelectConstructor_MultipleConstructors()
        {
            Type type = typeof(MultipleConstructorsClass);

            var selectedConstructor = ConstructorResolvers.MostArguments.SelectConstructor(type);

            var constructorParameter = Assert.Single(selectedConstructor.GetParameters());
            Assert.Equal(typeof(int), constructorParameter.ParameterType);
        }
    }
}