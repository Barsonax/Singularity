﻿using System;
using System.Linq.Expressions;
using Singularity.Expressions;
using Xunit;

namespace Singularity.Test
{
    public class TypeExtensionsTests
    {
        [Fact]
        public void AutoResolveConstructor_ValueTypeWithNoConstructors()
        {
            Type type = typeof(int);
            Expression expression = type.AutoResolveConstructorExpression(ConstructorSelectors.Default);
            Assert.IsType<DefaultExpression>(expression);
        }
    }
}
