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
                yield return new ServiceBinding(type, new BindingMetadata(), type.AutoResolveConstructorExpression());
            }
        }
    }
}