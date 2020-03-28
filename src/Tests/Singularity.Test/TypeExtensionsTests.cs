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
            Expression expression = new DefaultConstructorResolver().TryResolveConstructorExpression(type);
            Assert.IsType<DefaultExpression>(expression);
        }
    }
}
