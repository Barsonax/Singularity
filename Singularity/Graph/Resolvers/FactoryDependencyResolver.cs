using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Singularity.Graph.Resolvers
{
    internal sealed class FactoryDependencyResolver : IDependencyResolver
    {
        public IEnumerable<ServiceBinding> Resolve(IResolverPipeline graph, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Func<>))
            {
                Type dependencyType = type.GenericTypeArguments[0];

                foreach (InstanceFactory factory in graph.ResolveAll(dependencyType))
                {
                    LambdaExpression baseExpression = Expression.Lambda(factory.Expression);

                    yield return new ServiceBinding(type, new BindingMetadata(), baseExpression)
                    {
                        BaseExpression = baseExpression
                    };
                }
            }
        }
    }
}
