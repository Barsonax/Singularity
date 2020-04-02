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

        public Type Target(Type type)
        {
            return typeof(Func<>).MakeGenericType(type);
        }

        public Expression Wrap(Expression expression, Type unWrappedType, Type wrappedType)
        {
            return Expression.Lambda(wrappedType, expression);
        }
    }
}
