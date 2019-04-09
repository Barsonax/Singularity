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

        public Dependency Resolve(DependencyGraph graph, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Lazy<>))
            {
                var dependency = graph.TryGetDependency(type.GenericTypeArguments[0]);
                graph.ResolveDependency(dependency.Default);

                var method = GenericGenerateLazyDependencyMethod.MakeGenericMethod(type.GenericTypeArguments);
                Dependency lazyDependency = (Dependency)method.Invoke(null, new object[] { type, dependency });
                return lazyDependency;
            }

            return null;
        }

        private static Dependency GenerateLazyDependency<T>(Type type, Dependency dependency)
        {
            var constructor = typeof(Lazy<T>).GetConstructor(new[] { typeof(Func<T>) });
            Expression expression = Expression.New(constructor,
                Expression.Lambda(
                    Expression.Convert(
                        Expression.Call(dependency.Default.InstanceFactory!.Method, ExpressionGenerator.ScopeParameter), type.GenericTypeArguments[0])) );
            Dependency lazyDependency = new Dependency(type, expression, CreationMode.Transient);
            lazyDependency.Default.Expression = expression;
            lazyDependency.Default.InstanceFactory = scoped => new Lazy<T>(() => (T)dependency.Default.InstanceFactory!.Invoke(scoped));
            return lazyDependency;
        }
    }
}
