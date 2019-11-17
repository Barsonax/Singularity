using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Exceptions;
using Singularity.Expressions;
using Singularity.Graph.Resolvers;

namespace Singularity
{
    /// <summary>
    /// Assumes there is only 1 public constructor. Throws a error if there are multiple public constructors.
    /// If this is undesired take a look at other resolves in <see cref="ConstructorResolvers"/>/>
    /// </summary>
    public class DefaultConstructorResolver : IConstructorResolver
    {
        /// <summary>
        /// Tries to find a constructor.
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="NoConstructorException">If there is no public constructor</exception>
        /// <exception cref="CannotAutoResolveConstructorException">If there is more than 1 public constructors</exception>
        /// <returns></returns>
        public ConstructorInfo StaticSelectConstructor(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructorCandidates().ToArray();
            if (constructors.Length == 0 && !type.IsValueType) { throw new NoConstructorException($"Type {type} did not contain any public constructor."); }

            if (constructors.Length > 1)
            {
                throw new CannotAutoResolveConstructorException($"Found {constructors.Length} suitable constructors for type {type}. Please specify the constructor explicitly.");
            }
            return constructors.FirstOrDefault();
        }

        public ConstructorInfo DynamicSelectConstructor(Type type, IResolverPipeline resolverPipeline)
        {
            throw new NotImplementedException();
        }

        public Expression ResolveConstructorExpression(Type type, ConstructorInfo constructorInfo)
        {
            return type.ResolveConstructorExpression(constructorInfo);
        }

        internal DefaultConstructorResolver() { }
    }
}
