using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Singularity.Exceptions;
using Singularity.Expressions;
using Singularity.Resolving;

namespace Singularity
{
    /// <summary>
    /// A resolver that picks the constructor with the most arguments that can still be resolved.
    /// </summary>
    public class BestMatchConstructorResolver : IConstructorResolver
    {
        /// <inheritdoc />
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

        /// <inheritdoc />
        public ConstructorInfo DynamicSelectConstructor(Type type, IInstanceFactoryResolver instanceFactoryResolver)
        {
            ConstructorInfo[] constructors = type.GetConstructorCandidates().ToArray();
            if (constructors.Length == 0 && !type.IsValueType) { throw new NoConstructorException($"Type {type} did not contain any public constructor."); }

            if (constructors.Length > 1)
            {
                var ordering = constructors.OrderByDescending(x => x.GetParameters().Length);
                foreach (var constructorInfo in ordering)
                {
                    if (constructorInfo.GetParameters().All(x => instanceFactoryResolver.TryResolve(x.ParameterType) != null))
                    {
                        return constructorInfo;
                    }
                }
            }
            return constructors.FirstOrDefault();
        }

        /// <inheritdoc />
        public Expression? ResolveConstructorExpression(Type type, ConstructorInfo? constructorInfo)
        {
            if (constructorInfo == null) return null;
            return type.ResolveConstructorExpression(constructorInfo);
        }
    }
}
