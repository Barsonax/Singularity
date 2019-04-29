using System;
using System.Collections.Generic;
using Singularity.Bindings;

namespace Singularity.Graph.Resolvers
{
    internal interface IResolverPipeline
    {
        Registration? TryGetDependency(Type type);
        Registration GetDependency(Type type);
        InstanceFactory ResolveDependency(Type type, Binding dependency);
        IReadOnlyDictionary<Type, Registration> Dependencies { get; }
        object SyncRoot { get; }
    }
}