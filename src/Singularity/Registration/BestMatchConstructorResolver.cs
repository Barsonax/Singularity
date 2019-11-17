using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Exceptions;
using Singularity.Expressions;
using Singularity.Graph.Resolvers;

namespace Singularity
{
    public class BestMatchConstructorResolver : IConstructorResolver
    {
        public ConstructorInfo? StaticSelectConstructor(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructorCandidates().ToArray();
            if (constructors.Length == 0 && !type.IsValueType) { throw new NoConstructorException($"Type {type} did not contain any public constructor."); }

            if (constructors.Length > 1)
            {
                return null;
            }
            return constructors.Single();
        }

        public ConstructorInfo DynamicSelectConstructor(Type type, IResolverPipeline resolverPipeline)
        {
            ConstructorInfo[] constructors = type.GetConstructorCandidates().ToArray();
            if (constructors.Length == 0 && !type.IsValueType) { throw new NoConstructorException($"Type {type} did not contain any public constructor."); }

            if (constructors.Length > 1)
            {
                var ordering = constructors.OrderByDescending(x => x.GetParameters().Length);
                foreach (var constructorInfo in ordering)
                {
                    if (constructorInfo.GetParameters().All(x => resolverPipeline.TryResolve(x.ParameterType) != null))
                    {
                        return constructorInfo;
                    }
                }
            }
            return constructors.FirstOrDefault();
        }

        public Expression? ResolveConstructorExpression(Type type, ConstructorInfo constructorInfo)
        {
            if (constructorInfo == null) return null;
            return type.ResolveConstructorExpression(constructorInfo);
        }


        internal BestMatchConstructorResolver() { }
    }
}
