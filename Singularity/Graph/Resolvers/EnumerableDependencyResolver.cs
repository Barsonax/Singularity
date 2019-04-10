using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

                    IEnumerable<Dependency> collectionDependencies = new[] { enumerableType, collectionType, listType }.Select(x =>
                          new Dependency(x, expression, CreationMode.Transient)).ToArray();
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

    internal class InstanceFactoryList<T> : IReadOnlyList<T>
    {
        private readonly Func<Scoped, object>[] _instanceFactories;
        private readonly Scoped _scope;

        public InstanceFactoryList(Scoped scope, Func<Scoped, object>[] instanceFactories)
        {
            _instanceFactories = instanceFactories ?? throw new ArgumentNullException(nameof(instanceFactories));
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (Func<Scoped, object> instanceFactory in _instanceFactories)
            {
                yield return (T)instanceFactory.Invoke(_scope);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _instanceFactories.Length;

        public T this[int index] => (T)_instanceFactories[index].Invoke(_scope);
    }
}