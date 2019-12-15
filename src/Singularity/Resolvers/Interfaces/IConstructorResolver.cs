using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity.Resolvers
{
    public interface IConstructorResolver
    {
        /// <summary>
        /// Selects a constructor for a type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        ConstructorInfo? StaticSelectConstructor(Type type);

        /// <summary>
        /// Selects a constructor for a type.
        /// </summary>
        /// <param name="containerContext"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        ConstructorInfo DynamicSelectConstructor(Type type, IContainerContext containerContext);

        Expression? ResolveConstructorExpression(Type type, ConstructorInfo? constructorInfo);
    }

    public static class ConstructorResolverExtensions
    {
        public static Expression? ResolveConstructorExpression(this IConstructorResolver resolver, Type type)
        {
            return resolver.ResolveConstructorExpression(type, resolver.StaticSelectConstructor(type));
        }
    }
}