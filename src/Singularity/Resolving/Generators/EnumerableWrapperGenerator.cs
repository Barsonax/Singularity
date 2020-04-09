using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Singularity.Collections;

namespace Singularity.Resolving.Generators
{
    public class EnumerableWrapperGenerator : IGenericWrapperGenerator
    {
        public bool CanResolve(Type type) => type.IsGenericType && new[] { typeof(IEnumerable<>), typeof(IReadOnlyCollection<>), typeof(IReadOnlyList<>), }.Contains(type.GetGenericTypeDefinition());

        public Expression Wrap<TUnwrapped, TWrapped>() => ConstructorResolvers.Default.ResolveConstructorExpression(typeof(InstanceFactoryList<TUnwrapped>));

        public Type? DependsOn(Type type) => typeof(Func<,>).MakeGenericType(typeof(Scoped), type.GetGenericArguments()[0]).MakeArrayType();
    }
}