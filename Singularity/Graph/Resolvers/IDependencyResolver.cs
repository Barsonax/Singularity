using System;

namespace Singularity.Graph.Resolvers
{
    internal interface IDependencyResolver
    {
        Dependency? Resolve(DependencyGraph graph, Type type);
    }
}
