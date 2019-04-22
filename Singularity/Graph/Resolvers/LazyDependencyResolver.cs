using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Bindings;

namespace Singularity.Graph.Resolvers
{
    internal class LazyDependencyResolver : IDependencyResolver
    {
        public Dependency? Resolve(IResolverPipeline graph, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Lazy<>))
            {
                Type funcType = typeof(Func<>).MakeGenericType(type.GenericTypeArguments[0]);
                Type lazyType = typeof(Lazy<>).MakeGenericType(type.GenericTypeArguments[0]);
                Dependency factoryDependency = graph.GetDependency(funcType);

                var expressions = new List<Expression>();
                ConstructorInfo constructor = lazyType.GetConstructor(new[] { funcType });
                foreach (ResolvedDependency resolvedDependency in factoryDependency.ResolvedDependencies.Array)
                {
                    expressions.Add(Expression.New(constructor, graph.ResolveDependency(lazyType, resolvedDependency).Expression));
                }

                var lazyDependency = new Dependency(new[] { type }, expressions, Lifetime.Transient);
                for (var i = 0; i < lazyDependency.ResolvedDependencies.Array.Length; i++)
                {
                    lazyDependency.ResolvedDependencies.Array[i].Factories.Add(new InstanceFactory(type, expressions[i]));
                }
                return lazyDependency;
            }
            return null;
        }
    }
}
