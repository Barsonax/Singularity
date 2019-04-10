using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity.Graph.Resolvers
{
    internal class FactoryDependencyResolver : IDependencyResolver
    {
        private static readonly MethodInfo GenericCreateFactoryDependencyMethod;

        static FactoryDependencyResolver()
        {
            GenericCreateFactoryDependencyMethod = (from m in typeof(FactoryDependencyResolver).GetRuntimeMethods()
                                                    where m.Name == nameof(CreateFactoryDependency)
                                                    select m).Single();
        }

        public IEnumerable<Dependency>? Resolve(DependencyGraph graph, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Func<>))
            {
                var dependency = graph.GetDependency(type.GenericTypeArguments[0]);
                graph.ResolveDependency(dependency!.Default);

                MethodInfo method = GenericCreateFactoryDependencyMethod.MakeGenericMethod(type.GenericTypeArguments);
                var factoryDependency = (Dependency)method.Invoke(null, new object[] { type, dependency });
                return new[] { factoryDependency };
            }

            return null;
        }

        private static Dependency CreateFactoryDependency<T>(Type type, Dependency dependency)
        {
            LambdaExpression expression = Expression.Lambda(dependency.Default.Expression);
            var factoryDependency = new Dependency(type, expression, CreationMode.Transient);
            factoryDependency.Default.Expression = expression;
            return factoryDependency;
        }
    }
}
