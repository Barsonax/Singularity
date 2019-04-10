using System;
using System.Collections.Generic;

namespace Singularity.Graph.Resolvers
{
    internal class ConcreteDependencyResolver : IDependencyResolver
    {
        public IEnumerable<Dependency>? Resolve(DependencyGraph graph, Type type)
        {
            if (!type.IsInterface)
            {
                if (type.IsGenericType) return null;
                return new[] { new Dependency(type, CreationMode.Transient) };
            }

            return null;
        }
    }
}