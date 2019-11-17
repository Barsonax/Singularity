using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Expressions;

namespace Singularity
{
    public static class ConstructorResolverExtensions
    {
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

        public static Expression? ResolveConstructorExpression(this IConstructorResolver resolver, Type type)
        {
            return resolver.ResolveConstructorExpression(type, resolver.StaticSelectConstructor(type));
        }
    }
}