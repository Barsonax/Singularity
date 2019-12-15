using System;
using System.Collections.Generic;
using Singularity.Bindings;

namespace Singularity.Resolvers
{
    /// <summary>
    /// Interface for creating new bindings.
    /// Singularity uses this to provide advanced dependency injection features such as generic wrappers and open generics.
    /// </summary>
    public interface IServiceBindingGenerator
    {
        /// <summary>
        /// Dynamically creates bindings for a given type.
        /// If this returns <c>null</c> or a empty <see cref="IEnumerable{T}"/> then the next <see cref="IServiceBindingGenerator"/> in <see cref="SingularitySettings.ServiceBindingGenerators"/> will be used.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        IEnumerable<ServiceBinding> TryGenerate(IContainerContext context, Type type);
    }
}
