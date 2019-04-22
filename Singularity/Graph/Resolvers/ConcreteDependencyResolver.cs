using System;
using System.Collections.Generic;

namespace Singularity.Graph.Resolvers
{
    internal class ConcreteDependencyResolver : IDependencyResolver
    {
        public Dependency? Resolve(IResolverPipeline graph, Type type)
        {
            if (!type.IsInterface)
            {
                if (type.IsGenericType) return null;
                return new Dependency(new[] { type }, type.AutoResolveConstructorExpression(), Lifetime.Transient);
            }

            return null;
        }
    }
}