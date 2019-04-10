using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Expressions;

namespace Singularity.Graph.Resolvers
{
    internal class LazyDependencyResolver : IDependencyResolver
    {
        private static readonly MethodInfo GenericGenerateLazyDependencyMethod;

        static LazyDependencyResolver()
        {
            GenericGenerateLazyDependencyMethod = (from m in typeof(LazyDependencyResolver).GetRuntimeMethods()
                                                   where m.Name == nameof(GenerateLazyDependency)
                                                   select m).Single();
        }

        public Dependency? Resolve(DependencyGraph graph, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Lazy<>))
            {
                Type funcType = typeof(Func<>).MakeGenericType(type.GenericTypeArguments[0]);
                Dependency factoryDependency = graph.GetDependency(funcType);
                graph.ResolveDependency(factoryDependency.Default);

                MethodInfo method = GenericGenerateLazyDependencyMethod.MakeGenericMethod(type.GenericTypeArguments);
                var lazyDependency = (Dependency)method.Invoke(null, new object[] { type, factoryDependency });
                return lazyDependency;
            }

            return null;
        }

        private static Dependency GenerateLazyDependency<T>(Type type, Dependency factoryDependency)
        {
            ConstructorInfo constructor = typeof(Lazy<T>).GetConstructor(new[] { typeof(Func<T>) });
            Expression expression = Expression.New(constructor, factoryDependency.Default.Expression);
            var lazyDependency = new Dependency(type, expression, CreationMode.Transient);
            lazyDependency.Default.Expression = expression;
            return lazyDependency;
        }
    }
}
