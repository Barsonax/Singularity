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
                var dependency = graph.GetDependency(type.GenericTypeArguments[0]);
                graph.ResolveDependency(dependency!.Default);

                LambdaExpression expression = Expression.Lambda(dependency.Default.Expression);
                var factoryDependency = new Dependency(type, expression, CreationMode.Transient);
                factoryDependency.Default.Expression = expression;
                return new[] { factoryDependency };
            }

            return null;
        }
    }
}
