using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Singularity.Collections;
using Singularity.Expressions;

namespace Singularity.Graph.Resolvers
{
    internal sealed class EnumerableDependencyResolver : IDependencyResolver
    {
        public IEnumerable<ServiceBinding> Resolve(IResolverPipeline graph, Type type)
        {
            if (type.IsGenericType)
            {
                Type definition = type.GetGenericTypeDefinition();
                if (definition == typeof(IEnumerable<>) || definition == typeof(IReadOnlyCollection<>) || definition == typeof(IReadOnlyList<>))
                {
                    Type elementType = type.GenericTypeArguments[0];

                    Func<Scoped, object>[] instanceFactories = graph.TryResolveAll(elementType).Select(x => x.Factory).ToArray();

                    Type instanceFactoryListType = typeof(InstanceFactoryList<>).MakeGenericType(type.GenericTypeArguments);
                    Expression expression = Expression.New(ConstructorSelectors.Default.SelectConstructor(instanceFactoryListType), ExpressionGenerator.ScopeParameter, Expression.Constant(instanceFactories));

                    Type[] types = {
                        typeof(IEnumerable<>).MakeGenericType(elementType),
                        typeof(IReadOnlyCollection<>).MakeGenericType(elementType),
                        typeof(IReadOnlyList<>).MakeGenericType(elementType)
                    };

                    foreach (Type newType in types)
                    {
                        yield return new ServiceBinding(newType, BindingMetadata.GeneratedInstance, expression, graph.Settings.ConstructorSelector);
                    }
                }
            }
        }
    }
}