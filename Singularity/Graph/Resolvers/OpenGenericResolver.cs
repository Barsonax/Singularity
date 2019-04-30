using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Singularity.Expressions;

namespace Singularity.Graph.Resolvers
{
    internal sealed class OpenGenericResolver : IDependencyResolver
    {
        public IEnumerable<Binding> Resolve(IResolverPipeline graph, Type type)
        {
            if (type.IsGenericType && !type.ContainsGenericParameters)
            {
                Type genericTypeDefinition = type.GetGenericTypeDefinition();
                Registration? genericRegistration = graph.TryGetDependency(genericTypeDefinition);
                if (genericRegistration != null)
                {
                    Binding genericBinding = genericRegistration.Default;
                    Type openGenericType = ((OpenGenericTypeExpression)genericBinding.Expression!).Type;
                    Type closedGenericType = openGenericType.MakeGenericType(type.GenericTypeArguments);
                    Expression newExpression = closedGenericType.AutoResolveConstructorExpression();

                    yield return new Binding(new BindingMetadata(type), newExpression, genericBinding.Lifetime);
                }
            }
        }
    }
}