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
        public IEnumerable<Binding> Resolve(IResolverPipeline graph, Type type)
        {
            if (type.IsGenericType)
            {
                Type definition = type.GetGenericTypeDefinition();
                if (definition == typeof(IEnumerable<>) || definition == typeof(IReadOnlyCollection<>) || definition == typeof(IReadOnlyList<>))
                {
                    Type elementType = type.GenericTypeArguments[0];
                    Registration? registration = graph.TryGetDependency(elementType);
                    Binding[] dependencies = registration?.Bindings.Array ?? new Binding[0];

                    Func<Scoped, object>[] instanceFactories = dependencies.Select(x => graph.ResolveDependency(elementType, x).Factory).ToArray();

                    Type instanceFactoryListType = typeof(InstanceFactoryList<>).MakeGenericType(type.GenericTypeArguments);
                    Expression expression = Expression.New(instanceFactoryListType.AutoResolveConstructor(), ExpressionGenerator.ScopeParameter, Expression.Constant(instanceFactories));

                    Type[] types = {
                        typeof(IEnumerable<>).MakeGenericType(elementType),
                        typeof(IReadOnlyCollection<>).MakeGenericType(elementType),
                        typeof(IReadOnlyList<>).MakeGenericType(elementType)
                    };

                    foreach (Type newType in types)
                    {
                        yield return new Binding(new BindingMetadata(newType), expression);
                    }
                }
            }
        }
    }
}