using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Tries to create a <see cref="Expression"/> representing the call to the constructor or in case of value types it may be just a default.
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public static Expression ResolveConstructorExpression(this Type type, ConstructorInfo? constructor)
        {
            if (constructor == null && type.IsValueType)
            {
                return Expression.Default(type);
            }
            else if(constructor == null)
            {
                throw new ArgumentNullException(nameof(constructor), "You can only pass a null for the constructor argument if the type is a value type");
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

        public static IEnumerable<ConstructorInfo> GetConstructorCandidates(this Type type)
        {
            return type.GetTypeInfo().DeclaredConstructors
                .Where(x => x.IsPublic)
                .Where(x => x.GetParameters().All(p => !p.ParameterType.IsEnum &&
                                                       !p.ParameterType.IsPrimitive &&
                                                       p.ParameterType != typeof(string)));
        }
    }
}