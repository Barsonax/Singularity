using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Tries to create a <see cref="NewExpression"/> representing the call to the constructor.
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="NoConstructorException">If there is no public constructor</exception>
        /// <exception cref="CannotAutoResolveConstructorException">If there is more than 1 public constructors</exception>
        /// <returns></returns>
        public static NewExpression AutoResolveConstructor(this Type type)
        {
            var constructors = type.GetTypeInfo().DeclaredConstructors.Where(x => x.IsPublic).ToArray();
            if (constructors.Length == 0) throw new NoConstructorException($"Type {type} did not contain any public constructor.");
            if (constructors.Length > 1) throw new CannotAutoResolveConstructorException($"Found {constructors.Length} suitable constructors for type {type}. Please specify the constructor explicitly.");
            var constructor = constructors.First();
            var parameters = constructor.GetParameters();
            var parameterExpressions = new Expression[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                parameterExpressions[i] = Expression.Parameter(parameters[i].ParameterType);
            }
            return Expression.New(constructor, parameterExpressions);
        }
    }
}
