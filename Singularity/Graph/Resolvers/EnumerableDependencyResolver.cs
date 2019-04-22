using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Singularity.Collections;
using Singularity.Expressions;

namespace Singularity.Graph.Resolvers
{
    internal class EnumerableDependencyResolver : IDependencyResolver
    {
        public Dependency? Resolve(IResolverPipeline graph, Type type)
        {
            if (type.IsGenericType)
            {
                Type definition = type.GetGenericTypeDefinition();
                if (definition == typeof(IEnumerable<>) || definition == typeof(IReadOnlyCollection<>) || definition == typeof(IReadOnlyList<>))
                {
                    Type elementType = type.GenericTypeArguments[0];
                    Dependency? childDependency = graph.TryGetDependency(elementType);
                    ResolvedDependency[] dependencies = childDependency?.ResolvedDependencies.Array ?? new ResolvedDependency[0];

                    Func<Scoped, object>[] instanceFactories = dependencies.Select(x => graph.ResolveDependency(elementType, x).Factory).ToArray();

                    Type instanceFactoryListType = typeof(InstanceFactoryList<>).MakeGenericType(type.GenericTypeArguments);
                    Expression expression = Expression.New(instanceFactoryListType.AutoResolveConstructor(), ExpressionGenerator.ScopeParameter, Expression.Constant(instanceFactories));

                    Type enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);
                    Type collectionType = typeof(IReadOnlyCollection<>).MakeGenericType(elementType);
                    Type listType = typeof(IReadOnlyList<>).MakeGenericType(elementType);

                    var collectionDependency = new Dependency(new[] { enumerableType, collectionType, listType }, expression, Lifetime.Transient);

                    return collectionDependency;
                }
            }
            return null;
        }
    }
}