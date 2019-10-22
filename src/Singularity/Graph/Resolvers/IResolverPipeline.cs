using System;
using System.Collections.Generic;

namespace Singularity.Graph.Resolvers
{
    internal interface IResolverPipeline
    {
        SingularitySettings Settings { get; }
        InstanceFactory Resolve(Type type);
        IEnumerable<InstanceFactory> ResolveAll(Type type);
        InstanceFactory? TryResolve(Type type);
        IEnumerable<InstanceFactory> TryResolveAll(Type type);
    }
}