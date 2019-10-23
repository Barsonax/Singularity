using System;
using System.Linq;
using System.Reflection;
using Singularity.Exceptions;
using Singularity.Expressions;

namespace Singularity
{
    public class DefaultConstructorSelector : IConstructorSelector
    {
        /// <summary>
        /// Tries to find a constructor.
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="NoConstructorException">If there is no public constructor</exception>
        /// <exception cref="CannotAutoResolveConstructorException">If there is more than 1 public constructors</exception>
        /// <returns></returns>
        public ConstructorInfo SelectConstructor(Type type)
        {
            ConstructorInfo[] constructors = type.GetTypeInfo().DeclaredConstructors.Where(x => x.IsPublic).ToArray();
            if (constructors.Length == 0 && !type.IsValueType) { throw new NoConstructorException($"Type {type} did not contain any public constructor."); }

            if (constructors.Length > 1)
            {
                throw new CannotAutoResolveConstructorException($"Found {constructors.Length} suitable constructors for type {type}. Please specify the constructor explicitly.");
            }
            return constructors.FirstOrDefault();
        }

        internal DefaultConstructorSelector() { }
    }
}
