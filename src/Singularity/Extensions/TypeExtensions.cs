using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Tries to create a <see cref="Expression"/> representing the call to the constructor or in case of value types it may be just a default.
        /// </summary>
        /// <param name="constructor"></param>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public static Expression ResolveConstructorExpression(this Type type, ConstructorInfo constructor)
        {
            if (constructor == null && type.IsValueType)
            {
                return Expression.Default(type);
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