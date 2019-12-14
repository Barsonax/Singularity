using System;
using System.Collections.Generic;

namespace Singularity.Graph.Resolvers
{
    /// <summary>
    /// Creates a binding if the type is a concrete type.
    /// </summary>
    public sealed class ConcreteServiceBindingGenerator : IServiceBindingGenerator
    {
        /// <inheritdoc />
        public IEnumerable<ServiceBinding> Resolve(IResolverPipeline graph, Type type)
        {
            if (!type.IsInterface)
            {
                yield return new ServiceBinding(type, BindingMetadata.GeneratedInstance, graph.Settings.ConstructorResolver.ResolveConstructorExpression(type), type, graph.Settings.ConstructorResolver, Lifetimes.Transient);
            }
        }
    }
}