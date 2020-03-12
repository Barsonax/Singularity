using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Expressions;

namespace Singularity
{
    /// <summary>
    /// Extensions for <see cref="IConstructorResolver"/>
    /// </summary>
    public static class ConstructorResolverExtensions
    {
        /// <summary>
        /// Returns usable constructors for the passed <paramref name="type"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<ConstructorInfo> GetConstructorCandidates(this Type type)
        {
            return type.GetTypeInfo().DeclaredConstructors
                .Where(x => x.IsPublic)
                .Where(x => x.GetParameters().All(p =>
                {
                    return !p.ParameterType.IsEnum &&
                           !p.ParameterType.IsPrimitive &&
                           p.ParameterType != typeof(string);
                }));
        }

        /// <summary>
        /// Returns the expression to call the constructor that is selected by <paramref name="resolver"/> for the passed <paramref name="type"/>
        /// </summary>
        /// <param name="resolver"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Expression? ResolveConstructorExpression(this IConstructorResolver resolver, Type type)
        {
            return resolver.ResolveConstructorExpression(type, resolver.StaticSelectConstructor(type));
        }
    }
}