using System;
using System.Linq.Expressions;

using Singularity.Exceptions;
using Singularity.Test.TestClasses;
using Xunit;

namespace Singularity.Test
{
    public class TypeExtensionsTests
    {
        [Fact]
        public void AutoResolveConstructor_NoConstructors_Throws()
        {
            Type type = typeof(NoPublicConstructorClass);
            Assert.Throws<NoConstructorException>(() =>
            {
                NewExpression constructorExpression = type.AutoResolveConstructorExpression();
            });           
        }

        [Fact]
        public void AutoResolveConstructor_MultipleConstructors_Throws()
        {
            Type type = typeof(MultipleConstructorsClass);
            Assert.Throws<CannotAutoResolveConstructorException>(() =>
            {
                NewExpression constructorExpression = type.AutoResolveConstructorExpression();
            });
        }
    }
}
