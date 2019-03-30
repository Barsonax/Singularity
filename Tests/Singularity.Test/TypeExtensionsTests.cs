using System;
using System.Linq.Expressions;

using Singularity.Exceptions;
using Singularity.TestClasses.TestClasses;
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
                Expression constructorExpression = type.AutoResolveConstructorExpression();
            });
        }

        [Fact]
        public void AutoResolveConstructor_MultipleConstructors_Throws()
        {
            Type type = typeof(MultipleConstructorsClass);
            Assert.Throws<CannotAutoResolveConstructorException>(() =>
            {
                Expression constructorExpression = type.AutoResolveConstructorExpression();
            });
        }

        [Fact]
        public void AutoResolveConstructor_ValueTypeWithNoConstructors()
        {
            Type type = typeof(int);
            Expression expression = type.AutoResolveConstructorExpression();
            Assert.IsType<DefaultExpression>(expression);
        }
    }
}
