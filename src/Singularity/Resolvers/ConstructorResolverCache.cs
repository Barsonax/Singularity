using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity.Resolvers
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

        public ConstructorInfo DynamicSelectConstructor(Type type, IContainerContext containerContext)
        {
            return _constructorResolver.DynamicSelectConstructor(type, containerContext);
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