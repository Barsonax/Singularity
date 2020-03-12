using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

using Singularity.Expressions;
using Singularity.Resolving;

namespace Singularity
{
    /// <summary>
    /// Provides caching for <see cref="IConstructorResolver"/>
    /// </summary>
    public class ConstructorResolverCache : IConstructorResolver
    {
        private readonly ConcurrentDictionary<Type, ConstructorInfo?> StaticSelectConstructorCache = new ConcurrentDictionary<Type, ConstructorInfo?>();
        private readonly ConcurrentDictionary<Type, Expression?> ResolveConstructorExpressionCache = new ConcurrentDictionary<Type, Expression?>();
        private readonly IConstructorResolver _constructorResolver;

        /// <summary>
        /// Wraps around the passed <paramref name="constructorResolver"></paramref> to add caching.
        /// </summary>
        /// <param name="constructorResolver"></param>
        public ConstructorResolverCache(IConstructorResolver constructorResolver)
        {
            _constructorResolver = constructorResolver;
        }

        /// <inheritdoc />
        public ConstructorInfo DynamicSelectConstructor(Type type, IInstanceFactoryResolver instanceFactoryResolver)
        {
            return _constructorResolver.DynamicSelectConstructor(type, instanceFactoryResolver);
        }

        /// <inheritdoc />
        public ConstructorInfo? StaticSelectConstructor(Type type)
        {
            return StaticSelectConstructorCache.GetOrAdd(type, t => _constructorResolver.StaticSelectConstructor(t));
        }

        /// <inheritdoc />
        public Expression? ResolveConstructorExpression(Type type, ConstructorInfo? constructorInfo)
        {
            return ResolveConstructorExpressionCache.GetOrAdd(type, t => _constructorResolver.ResolveConstructorExpression(t, constructorInfo));
        }

        /// <summary>
        /// Removes a type from the cache.
        /// </summary>
        /// <param name="type"></param>
        public void Remove(Type type)
        {
            StaticSelectConstructorCache.TryRemove(type, out _);
            ResolveConstructorExpressionCache.TryRemove(type, out _);
        }
    }
}