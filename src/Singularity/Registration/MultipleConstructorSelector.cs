using System;
using System.Linq;
using System.Reflection;
using Singularity.Exceptions;
using Singularity.Expressions;

namespace Singularity
{
    public class MultipleConstructorSelector : IConstructorSelector
    {
        /// <summary>
        /// Tries to find a constructor.
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="NoConstructorException">If there is no public constructor</exception>
        /// <returns></returns>
        public ConstructorInfo SelectConstructor(Type type)
        {
            ConstructorInfo[] constructors = type.GetTypeInfo().DeclaredConstructors.Where(x => x.IsPublic).ToArray();
            if (constructors.Length == 0 && !type.IsValueType) { throw new NoConstructorException($"Type {type} did not contain any public constructor."); }

            if (constructors.Length > 1)
            {
                return constructors.OrderByDescending(x => x.GetParameters().Length).FirstOrDefault();
            }
            return constructors.FirstOrDefault();
        }

        internal MultipleConstructorSelector() { }
    }
}