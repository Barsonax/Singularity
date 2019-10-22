using System;
using System.Reflection;

namespace Singularity.Expressions
{
    public interface IConstructorSelector
    {
        /// <summary>
        /// Selects a constructor for a type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        ConstructorInfo SelectConstructor(Type type);
    }
}