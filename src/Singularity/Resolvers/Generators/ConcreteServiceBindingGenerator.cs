using System;
using System.Collections.Generic;
using System.Linq;

namespace Singularity.Resolvers.Generators
{
    /// <summary>
    /// Creates a binding if the type is a concrete type.
    /// </summary>
    public sealed class ConcreteServiceBindingGenerator : IServiceBindingGenerator
    {
        /// <inheritdoc />
        public IEnumerable<ServiceBinding> Resolve(IInstanceFactoryResolver resolver, Type type)
        {
            if (!type.IsInterface && !type.IsAbstract && !type.IsPrimitive && type != typeof(string))
            {
                if (type.GetConstructorCandidates().Any())
                {
                    yield return new ServiceBinding(type, BindingMetadata.GeneratedInstance, resolver.Settings.ConstructorResolver.ResolveConstructorExpression(type), type, resolver.Settings.ConstructorResolver, Lifetimes.Transient);
                }
            }
        }
    }
}