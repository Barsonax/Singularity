using System;
using System.Linq.Expressions;
using Xunit;

namespace Singularity.Test
{
    public class TypeExtensionsTests
    {
        [Fact]
        public void AutoResolveConstructor_ValueTypeWithNoConstructors()
        {
            Type type = typeof(int);
            Expression expression = ConstructorResolvers.Default.AutoResolveConstructorExpression(type);
            Assert.IsType<DefaultExpression>(expression);
        }
    }
}
