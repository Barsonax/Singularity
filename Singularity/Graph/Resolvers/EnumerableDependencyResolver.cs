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
        public IEnumerable<Dependency>? Resolve(DependencyGraph graph, Type type)
        {
            if (type.IsGenericType)
            {
                Type definition = type.GetGenericTypeDefinition();
                if (definition == typeof(IEnumerable<>) || definition == typeof(IReadOnlyCollection<>) || definition == typeof(IReadOnlyList<>))
                {
                    Dependency childDependency = graph.TryGetDependency(type.GenericTypeArguments[0]);
                    ResolvedDependency[] dependencies = childDependency?.ResolvedDependencies.Array ?? new ResolvedDependency[0];
                    foreach (ResolvedDependency dependency in dependencies)
                    {
                        graph.ResolveDependency(dependency);
                    }

                    Func<Scoped, object>[] instanceFactories = dependencies.Select(x => x.InstanceFactory!).ToArray();

                    Type instanceFactoryListType = typeof(InstanceFactoryList<>).MakeGenericType(type.GenericTypeArguments);
                    Expression expression = Expression.New(instanceFactoryListType.AutoResolveConstructor(), ExpressionGenerator.ScopeParameter, Expression.Constant(instanceFactories));

                    Type enumerableType = typeof(IEnumerable<>).MakeGenericType(type.GenericTypeArguments[0]);
                    Type collectionType = typeof(IReadOnlyCollection<>).MakeGenericType(type.GenericTypeArguments[0]);
                    Type listType = typeof(IReadOnlyList<>).MakeGenericType(type.GenericTypeArguments[0]);

                    IEnumerable<Dependency> collectionDependencies = new[] { enumerableType, collectionType, listType }.Select(t => new Dependency(t, expression, CreationMode.Transient)).ToArray();
                    foreach (Dependency collectionDependency in collectionDependencies)
                    {
                        collectionDependency.Default.Expression = expression;
                    }

                    return collectionDependencies;
                }
            }
            return null;
        }
    }
}