using System;
using System.Collections.Generic;

namespace Singularity.Graph.Resolvers
{
    /// <summary>
    /// Interface for creating new bindings.
    /// Singularity uses this to provide advanced dependency injection features such as generic wrappers and open generics.
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
