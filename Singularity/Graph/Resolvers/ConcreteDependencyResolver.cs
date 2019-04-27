using System;
using System.Collections.Generic;

namespace Singularity.Graph.Resolvers
{
    internal class ConcreteDependencyResolver : IDependencyResolver
    {
        public IEnumerable<Binding> Resolve(IResolverPipeline graph, Type type)
        {
            if (!type.IsInterface && !type.IsGenericType)
            {
                yield return new Binding(new BindingMetadata(type), type.AutoResolveConstructorExpression());
            }
        }
    }
}