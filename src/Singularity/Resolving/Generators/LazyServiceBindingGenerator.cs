using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity.Resolving.Generators
{
    /// <summary>
    /// Creates bindings so that the binding can be resolved as a <see cref="Lazy{T}"/>
    /// </summary>
    public sealed class LazyServiceBindingGenerator : IGenericWrapperGenerator
    {
        public bool CanResolve(Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Lazy<>);

        public Type DependsOn(Type type) => typeof(Func<>).MakeGenericType(type);

        public Expression Wrap<TUnwrapped, TWrapped>(Expression expression, Type unWrappedType)
        {
            ConstructorInfo constructor = typeof(TWrapped).GetConstructor(new[] { unWrappedType });

            return Expression.New(constructor, expression);
        }
    }
}
