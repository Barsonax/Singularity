using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Singularity.Graph.Resolvers
{
    internal class FactoryDependencyResolver : IDependencyResolver
    {
        public Dependency? Resolve(IResolverPipeline graph, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Func<>))
            {
                Type dependencyType = type.GenericTypeArguments[0];
                Dependency dependency = graph.GetDependency(dependencyType);

                var expressions = new List<Expression>();
                foreach (ResolvedDependency resolvedDependency in dependency.ResolvedDependencies.Array)
                {
                    InstanceFactory factory = graph.ResolveDependency(dependencyType, resolvedDependency);
                    expressions.Add(Expression.Lambda(factory.Expression));
                }

                var factoryDependency = new Dependency(new []{ type }, expressions, Lifetime.Transient);
                for (var i = 0; i < factoryDependency.ResolvedDependencies.Array.Length; i++)
                {
                    factoryDependency.ResolvedDependencies.Array[i].BaseExpression = expressions[i];
                }
                return factoryDependency;
            }

            return null;
        }
    }
}
