using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Singularity.Expressions;

namespace Singularity.Graph.Resolvers
{
    /// <summary>
    /// Creates bindings so that open generic registrations can be properly resolved.
    /// </summary>
    public sealed class OpenGenericResolver : IServiceBindingGenerator
    {
        /// <inheritdoc />
        public IEnumerable<ServiceBinding> Resolve(IResolverPipeline graph, Type type)
        {
            if (type.IsGenericType && !type.ContainsGenericParameters)
            {
                Type genericTypeDefinition = type.GetGenericTypeDefinition();
                InstanceFactory? openGenericFactory = graph.TryResolve(genericTypeDefinition);
                if (openGenericFactory != null)
                {
                    var openGenericBinding = (ServiceBinding) ((ConstantExpression) openGenericFactory.Context.Expression).Value;

                    Type openGenericType = ((AbstractBindingExpression) openGenericBinding.Expression!).Type;
                    Type closedGenericType = openGenericType.MakeGenericType(type.GenericTypeArguments);
                    Expression newExpression = openGenericBinding.ConstructorResolver.AutoResolveConstructorExpression(closedGenericType);

                    yield return new ServiceBinding(type, openGenericBinding.BindingMetadata, newExpression,
                        openGenericBinding.ConstructorResolver, openGenericBinding.Lifetime, openGenericBinding.Finalizer,
                        openGenericBinding.NeedsDispose);
                }
            }
        }
    }
}