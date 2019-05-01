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
                InstanceFactory openGenericFactory = graph.Resolve(genericTypeDefinition);
                var openGenericBinding = (ServiceBinding) ((ConstantExpression) openGenericFactory.Expression).Value;

                Type openGenericType = ((AbstractBindingExpression)openGenericBinding.Expression!).Type;
                Type closedGenericType = openGenericType.MakeGenericType(type.GenericTypeArguments);
                Expression newExpression = closedGenericType.AutoResolveConstructorExpression();

                yield return new ServiceBinding(type, openGenericBinding.BindingMetadata, newExpression, openGenericBinding.Lifetime, openGenericBinding.Finalizer, openGenericBinding.NeedsDispose);
            }
        }
    }
}