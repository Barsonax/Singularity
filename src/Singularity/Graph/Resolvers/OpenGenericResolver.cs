using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Singularity.Expressions;

namespace Singularity.Graph.Resolvers
{
    internal sealed class OpenGenericResolver : IDependencyResolver
    {
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
                    Expression newExpression = openGenericBinding.ConstructorSelector.AutoResolveConstructorExpression(closedGenericType);

                    yield return new ServiceBinding(type, openGenericBinding.BindingMetadata, newExpression,
                        openGenericBinding.ConstructorSelector, openGenericBinding.Lifetime, openGenericBinding.Finalizer,
                        openGenericBinding.NeedsDispose);
                }
            }
        }
    }
}