using System;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Exceptions;
using Singularity.Expressions;

namespace Singularity
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Tries to create a <see cref="Expression"/> representing the call to the constructor or in case of value types it may be just a default.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="constructorSelector"></param>
        /// <exception cref="NoConstructorException">If there is no public constructor</exception>
        /// <exception cref="CannotAutoResolveConstructorException">If there is more than 1 public constructors</exception>
        /// <returns></returns>
        public static Expression AutoResolveConstructorExpression(this Type type, IConstructorSelector constructorSelector)
        {
            ConstructorInfo constructor = constructorSelector.SelectConstructor(type);

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
    }
}