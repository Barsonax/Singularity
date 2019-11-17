using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Expressions;
using Singularity.Graph.Resolvers;

namespace Singularity
{
    public class ConstructorResolverCache : IConstructorResolver
    {
        private readonly ConcurrentDictionary<Type, ConstructorInfo?> StaticSelectConstructorCache = new ConcurrentDictionary<Type, ConstructorInfo?>();
        private readonly ConcurrentDictionary<Type, Expression?> ResolveConstructorExpressionCache = new ConcurrentDictionary<Type, Expression?>();
        private readonly IConstructorResolver _constructorResolver;

        public ConstructorResolverCache(IConstructorResolver constructorResolver)
        {
            _constructorResolver = constructorResolver;
        }

        public ConstructorInfo DynamicSelectConstructor(Type type, IResolverPipeline resolverPipeline)
        {
            return _constructorResolver.DynamicSelectConstructor(type, resolverPipeline);
        }

        public ConstructorInfo? StaticSelectConstructor(Type type)
        {
            return StaticSelectConstructorCache.GetOrAdd(type, t => _constructorResolver.StaticSelectConstructor(t));
        }

        public Expression? ResolveConstructorExpression(Type type, ConstructorInfo constructorInfo)
        {
            return ResolveConstructorExpressionCache.GetOrAdd(type, t => _constructorResolver.ResolveConstructorExpression(t, constructorInfo));
        }
    }
}