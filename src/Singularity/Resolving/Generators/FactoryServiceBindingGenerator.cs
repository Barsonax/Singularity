using System;
using System.Linq.Expressions;

namespace Singularity.Resolving.Generators
{
    /// <summary>
    /// Creates bindings so that the factory itself of a binding can be resolved
    /// </summary>
    public sealed class FactoryServiceBindingGenerator : IGenericWrapperGenerator
    {
        public bool CanResolve(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Func<>);
        }

        public Type? DependsOn(Type type) => null;

        public Expression Wrap<TUnwrapped, TWrapped>(Expression expression, Type unWrappedType)
        {
            return Expression.Lambda<TWrapped>(expression);
        }
    }
}
