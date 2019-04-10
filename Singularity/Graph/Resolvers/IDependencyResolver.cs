using System;
using System.Collections.Generic;

namespace Singularity.Graph.Resolvers
{
    internal interface IDependencyResolver
    {
        IEnumerable<Dependency>? Resolve(DependencyGraph graph, Type type);
    }
}
