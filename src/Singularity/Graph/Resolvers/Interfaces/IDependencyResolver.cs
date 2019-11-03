using System;
using System.Collections.Generic;

namespace Singularity.Graph.Resolvers
{
    /// <summary>
    /// Interface for creating new bindings.
    /// </summary>
    public interface IDependencyResolver
    {
        /// <summary>
        /// Dynamically creates bindings for a given type.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        IEnumerable<ServiceBinding> Resolve(IResolverPipeline graph, Type type);
    }
}
