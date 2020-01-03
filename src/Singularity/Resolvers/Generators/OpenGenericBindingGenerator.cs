﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Singularity.Resolvers.Generators
{
    /// <summary>
    /// Creates bindings so that open generic registrations can be properly resolved.
    /// </summary>
    public sealed class OpenGenericBindingGenerator : IServiceBindingGenerator
    {
        /// <inheritdoc />
        public IEnumerable<ServiceBinding> Resolve(IInstanceFactoryResolver resolver, Type type)
        {
            if (type.IsGenericType && !type.ContainsGenericParameters)
            {
                Type genericTypeDefinition = type.GetGenericTypeDefinition();
                ServiceBinding? openGenericBinding = resolver.TryGetBinding(genericTypeDefinition);
                if (openGenericBinding != null)
                {
                    Type openGenericType = openGenericBinding.ConcreteType;
                    Type closedGenericType = openGenericType.MakeGenericType(type.GenericTypeArguments);
                    Expression? newExpression = openGenericBinding.ConstructorResolver.ResolveConstructorExpression(closedGenericType);

                    yield return new ServiceBinding(type, openGenericBinding.BindingMetadata, newExpression, closedGenericType,
                        openGenericBinding.ConstructorResolver, openGenericBinding.Lifetime, openGenericBinding.Finalizer,
                        openGenericBinding.NeedsDispose);
                }
            }
        }
    }
}