using System;
using System.Collections.Generic;
using Singularity.Bindings;
using Singularity.Resolvers;

namespace Singularity.Generators
{
    /// <summary>
    /// Creates a binding if the type is a concrete type.
    /// </summary>
    public sealed class ConcreteServiceBindingGenerator : IServiceBindingGenerator
    {
        /// <inheritdoc />
        public IEnumerable<ServiceBinding> TryGenerate(IContainerContext context, Type type)
        {
            if (!type.IsInterface)
            {
                yield return new ServiceBinding(type, BindingMetadata.GeneratedInstance, context.Settings.ConstructorResolver.ResolveConstructorExpression(type), type, context.Settings.ConstructorResolver, Lifetimes.Transient);
            }
        }
    }
}