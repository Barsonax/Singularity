using System;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Graph.Resolvers;

namespace Singularity.Expressions
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
        /// <param name="resolverPipeline"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        ConstructorInfo DynamicSelectConstructor(Type type, IResolverPipeline resolverPipeline);

        Expression? ResolveConstructorExpression(Type type, ConstructorInfo constructorInfo);
    }
}