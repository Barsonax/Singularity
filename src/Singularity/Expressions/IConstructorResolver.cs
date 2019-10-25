using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity.Expressions
{
    public interface IConstructorResolver
    {
        /// <summary>
        /// Selects a constructor for a type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        ConstructorInfo SelectConstructor(Type type);

        Expression AutoResolveConstructorExpression(Type type);
    }
}