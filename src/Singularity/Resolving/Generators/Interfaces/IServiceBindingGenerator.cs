using System;
using System.Collections.Generic;

namespace Singularity.Resolving.Generators
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
        /// <param name="resolver"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        IEnumerable<ServiceBinding> Resolve(IInstanceFactoryResolver resolver, Type type);
    }
}
