using System;
using System.Linq.Expressions;
using System.Reflection;

using Singularity.Resolving;

namespace Singularity.Expressions
{
    public interface IConstructorResolver
    {
        /// <summary>
        /// Selects a constructor for a type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        ConstructorInfo? StaticSelectConstructor(Type type);

        /// <summary>
        /// Selects a constructor for a type.
        /// </summary>
        /// <param name="instanceFactoryResolver"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        ConstructorInfo DynamicSelectConstructor(Type type, IInstanceFactoryResolver instanceFactoryResolver);

        Expression? ResolveConstructorExpression(Type type, ConstructorInfo? constructorInfo);
    }
}