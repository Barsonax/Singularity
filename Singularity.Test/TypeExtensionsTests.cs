using Singularity.Test.TestClasses;
using Xunit;

namespace Singularity.Test
{
    public class TypeExtensionsTests
    {
        [Fact]
        public void AutoResolveConstructor_NoConstructors_Throws()
        {
            var type = typeof(NoPublicConstructorClass);
            Assert.Throws<NoConstructorException>(() =>
            {
                var constructorExpression = type.AutoResolveConstructor();
            });           
        }

        [Fact]
        public void AutoResolveConstructor_MultipleConstructors_Throws()
        {
            var type = typeof(MultipleConstructorsClass);
            Assert.Throws<CannotAutoResolveConstructorException>(() =>
            {
                var constructorExpression = type.AutoResolveConstructor();
            });
        }
    }
}
