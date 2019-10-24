﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Exceptions;
using Singularity.Expressions;

namespace Singularity
{
    public class DefaultConstructorSelector : IConstructorSelector
    {
        public static ConcurrentDictionary<Type, ConstructorInfo> ConstructorInfoCache = new ConcurrentDictionary<Type, ConstructorInfo>();
        public static ConcurrentDictionary<Type, Expression> ExpressionCache = new ConcurrentDictionary<Type, Expression>();

        /// <summary>
        /// Tries to find a constructor.
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="NoConstructorException">If there is no public constructor</exception>
        /// <exception cref="CannotAutoResolveConstructorException">If there is more than 1 public constructors</exception>
        /// <returns></returns>
        public ConstructorInfo SelectConstructor(Type type)
        {
            return ConstructorInfoCache.GetOrAdd(type, t =>
            {
                ConstructorInfo[] constructors = t.GetTypeInfo().DeclaredConstructors.Where(x => x.IsPublic).ToArray();
                if (constructors.Length == 0 && !t.IsValueType) { throw new NoConstructorException($"Type {t} did not contain any public constructor."); }

                if (constructors.Length > 1)
                {
                    throw new CannotAutoResolveConstructorException($"Found {constructors.Length} suitable constructors for type {t}. Please specify the constructor explicitly.");
                }
                return constructors.FirstOrDefault();
            });
        }

        public Expression AutoResolveConstructorExpression(Type type)
        {
            return ExpressionCache.GetOrAdd(type, t => t.AutoResolveConstructorExpression(SelectConstructor(type)));
        }

        internal DefaultConstructorSelector() { }
    }
}
