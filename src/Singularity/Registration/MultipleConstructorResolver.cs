using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Exceptions;
using Singularity.Expressions;

namespace Singularity
{
    /// <summary>
    /// Picks the public constructor with the most arguments.
    /// </summary>
    public class MultipleConstructorResolver : IConstructorResolver
    {
        private static ConcurrentDictionary<Type, ConstructorInfo> Cache = new ConcurrentDictionary<Type, ConstructorInfo>();
        private static ConcurrentDictionary<Type, Expression> ExpressionCache = new ConcurrentDictionary<Type, Expression>();

        /// <summary>
        /// Tries to find a constructor.
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="NoConstructorException">If there is no public constructor</exception>
        /// <returns></returns>
        public ConstructorInfo SelectConstructor(Type type)
        {
            return Cache.GetOrAdd(type, t =>
            {
                ConstructorInfo[] constructors = t.GetTypeInfo().DeclaredConstructors.Where(x => x.IsPublic).ToArray();
                if (constructors.Length == 0 && !t.IsValueType) { throw new NoConstructorException($"Type {t} did not contain any public constructor."); }

                if (constructors.Length > 1)
                {
                    return constructors.OrderByDescending(x => x.GetParameters().Length).FirstOrDefault();
                }
                return constructors.FirstOrDefault();
            });
        }

        public Expression AutoResolveConstructorExpression(Type type)
        {
            return ExpressionCache.GetOrAdd(type, t => t.AutoResolveConstructorExpression(SelectConstructor(type)));
        }

        internal MultipleConstructorResolver() { }
    }
}