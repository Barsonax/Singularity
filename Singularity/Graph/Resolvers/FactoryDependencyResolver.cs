using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Singularity.Graph.Resolvers
{
    internal class FactoryDependencyResolver : IDependencyResolver
    {
        public IEnumerable<Dependency>? Resolve(DependencyGraph graph, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Func<>))
            {
                Dependency dependency = graph.GetDependency(type.GenericTypeArguments[0]);

                var expressions = new List<Expression>();
                foreach (ResolvedDependency resolvedDependency in dependency.ResolvedDependencies.Array)
                {
                    graph.ResolveDependency(resolvedDependency);
                    expressions.Add(Expression.Lambda(resolvedDependency.Expression));
                }

                var factoryDependency = new Dependency(type, expressions, CreationMode.Transient);
                for (int i = 0; i < factoryDependency.ResolvedDependencies.Array.Length; i++)
                {
                    factoryDependency.ResolvedDependencies.Array[i].Expression = expressions[i];
                }
                return new[] { factoryDependency };
            }

            return null;
        }
    }
}
