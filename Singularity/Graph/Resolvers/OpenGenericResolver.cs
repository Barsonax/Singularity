using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Singularity.Expressions;

namespace Singularity.Graph.Resolvers
{
    internal class OpenGenericResolver : IDependencyResolver
    {
        public IEnumerable<Dependency>? Resolve(IResolverPipeline graph, Type type)
        {
            if (type.IsGenericType && !type.ContainsGenericParameters)
            {
                Type genericTypeDefinition = type.GetGenericTypeDefinition();
                Dependency? genericDependency = graph.TryGetDependency(genericTypeDefinition);
                if (genericDependency != null)
                {
                    ResolvedDependency openGenericDependency = genericDependency.Default;
                    Type openGenericType = ((OpenGenericTypeExpression)openGenericDependency.Binding.Expression!).OpenGenericType;
                    Type closedGenericType = openGenericType.MakeGenericType(type.GenericTypeArguments);
                    Expression newExpression = closedGenericType.AutoResolveConstructorExpression();

                    var dependency = new Dependency(type, newExpression, openGenericDependency.Binding.Lifetime);

                    return new[] { dependency };
                }
            }

            return null;
        }
    }
}