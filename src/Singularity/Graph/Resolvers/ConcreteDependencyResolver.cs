using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Singularity.Graph.Resolvers
{
    internal sealed class ConcreteDependencyResolver : IDependencyResolver
    {
        public IEnumerable<ServiceBinding> Resolve(IResolverPipeline graph, Type type)
        {
            if (!type.IsInterface)
            {
                yield return new ServiceBinding(type, BindingMetadata.GeneratedInstance, type.AutoResolveConstructorExpression(graph.Settings.ConstructorSelector), graph.Settings.ConstructorSelector);
            }
        }
    }
}