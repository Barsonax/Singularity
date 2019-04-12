using System;
using System.Collections.Generic;

namespace Singularity.Graph.Resolvers
{
    internal interface IDependencyResolver
    {
        IEnumerable<Dependency>? Resolve(IResolverPipeline graph, Type type);
    }
}
