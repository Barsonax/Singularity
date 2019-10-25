using System;
using System.Collections.Generic;

namespace Singularity.Graph.Resolvers
{
    internal sealed class ConcreteDependencyResolver : IDependencyResolver
    {
        public IEnumerable<ServiceBinding> Resolve(IResolverPipeline graph, Type type)
        {
            if (!type.IsInterface)
            {
                yield return new ServiceBinding(type, BindingMetadata.GeneratedInstance, graph.Settings.ConstructorResolver.AutoResolveConstructorExpression(type), graph.Settings.ConstructorResolver);
            }
        }
    }
}