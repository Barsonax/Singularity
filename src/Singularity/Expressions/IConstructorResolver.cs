using System;
using System.Linq.Expressions;
using System.Reflection;

using Singularity.Resolving;

namespace Singularity.Expressions
{
    /// <summary>
    /// Interface for classes that can select a constructor.
    /// </summary>
    public interface IConstructorResolver
    {
        /// <summary>
        /// Selects a constructor for a type. Called when registering a service.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        ConstructorInfo? StaticSelectConstructor(Type type);

        /// <summary>
        /// Selects a constructor for a type. Called when resolving a service, allowing additional context information to be used.
        /// </summary>
        /// <param name="instanceFactoryResolver"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        ConstructorInfo DynamicSelectConstructor(Type type, IInstanceFactoryResolver instanceFactoryResolver);

        /// <summary>
        /// Resolves the constructor to a expression.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="constructorInfo"></param>
        /// <returns></returns>
        Expression? ResolveConstructorExpression(Type type, ConstructorInfo? constructorInfo);
    }
}
