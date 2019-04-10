using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity.Graph.Resolvers
{
    internal class LazyDependencyResolver : IDependencyResolver
    {
        public IEnumerable<Dependency>? Resolve(DependencyGraph graph, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Lazy<>))
            {
                Type funcType = typeof(Func<>).MakeGenericType(type.GenericTypeArguments[0]);
                Type lazyType = typeof(Lazy<>).MakeGenericType(type.GenericTypeArguments[0]);
                Dependency factoryDependency = graph.GetDependency(funcType);
                graph.ResolveDependency(factoryDependency.Default);

                ConstructorInfo constructor = lazyType.GetConstructor(new[] { funcType });
                Expression expression = Expression.New(constructor, factoryDependency.Default.Expression);
                var lazyDependency = new Dependency(type, expression, CreationMode.Transient);
                lazyDependency.Default.Expression = expression;
                return new[] { lazyDependency };
            }

            return null;
        }
    }
}
