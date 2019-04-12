using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity.Graph.Resolvers
{
    internal class LazyDependencyResolver : IDependencyResolver
    {
        public IEnumerable<Dependency>? Resolve(IResolverPipeline graph, Type type)
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
                    graph.ResolveDependency(resolvedDependency);
                    expressions.Add(Expression.New(constructor, resolvedDependency.Expression));
                }

                var lazyDependency = new Dependency(type, expressions, CreationMode.Transient);
                for (int i = 0; i < lazyDependency.ResolvedDependencies.Array.Length; i++)
                {
                    lazyDependency.ResolvedDependencies.Array[i].Expression = expressions[i];
                }
                return new[] { lazyDependency };
            }

            return null;
        }
    }
}
