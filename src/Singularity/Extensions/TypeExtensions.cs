using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Exceptions;

namespace Singularity
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Tries to create a <see cref="Expression"/> representing the call to the constructor or in case of value types it may be just a default.
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="NoConstructorException">If there is no public constructor</exception>
        /// <exception cref="CannotAutoResolveConstructorException">If there is more than 1 public constructors</exception>
        /// <returns></returns>
        public static Expression AutoResolveConstructorExpression(this Type type)
        {
            ConstructorInfo constructor = AutoResolveConstructor(type);
            if (constructor == null && type.IsValueType)
            {
                return Expression.Default(type);
            }
            else if (constructor == null)
            {
                throw new NotSupportedException();
            }
            ParameterInfo[] parameters = constructor.GetParameters();
            if (parameters.Length == 0) return Expression.New(constructor);
            var parameterExpressions = new Expression[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                parameterExpressions[i] = Expression.Parameter(parameters[i].ParameterType);
            }
            return Expression.New(constructor, parameterExpressions);
        }

        /// <summary>
        /// Tries to find a constructor.
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="NoConstructorException">If there is no public constructor</exception>
        /// <exception cref="CannotAutoResolveConstructorException">If there is more than 1 public constructors</exception>
        /// <returns></returns>
        public static ConstructorInfo AutoResolveConstructor(this Type type)
        {
            ConstructorInfo[] constructors = type.GetTypeInfo().DeclaredConstructors.Where(x => x.IsPublic).ToArray();
            if (constructors.Length == 0 && !type.IsValueType) { throw new NoConstructorException($"Type {type} did not contain any public constructor."); }
            if (constructors.Length > 1) throw new CannotAutoResolveConstructorException($"Found {constructors.Length} suitable constructors for type {type}. Please specify the constructor explicitly.");
            return constructors.FirstOrDefault();
        }
    }


    internal static class AutoResolveConstructorExpressionCache<T>
    {
        public static readonly Expression Expression = typeof(T).AutoResolveConstructorExpression();
    }
}
