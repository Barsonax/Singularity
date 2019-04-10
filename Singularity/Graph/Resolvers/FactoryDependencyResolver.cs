using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Expressions;

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

        public Dependency? Resolve(DependencyGraph graph, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Func<>))
            {
                var dependency = graph.TryGetDependency(type.GenericTypeArguments[0]);
                graph.ResolveDependency(dependency!.Default);

                var method = GenericCreateFactoryDependencyMethod.MakeGenericMethod(type.GenericTypeArguments);
                var factoryDependency = (Dependency)method.Invoke(null, new object[] { type, dependency });
                return factoryDependency;
            }

            return null;
        }

        private static Dependency CreateFactoryDependency<T>(Type type, Dependency dependency)
        {
            var expression = Expression.Lambda(
                Expression.Convert(
                    Expression.Call(dependency.Default.InstanceFactory!.Method, ExpressionGenerator.ScopeParameter), type.GenericTypeArguments[0]));

            var factoryDependency = new Dependency(type, expression, CreationMode.Transient);
            factoryDependency.Default.Expression = expression;
            factoryDependency.Default.InstanceFactory = scoped =>
            {
                T Func() => (T) dependency.Default.InstanceFactory!.Invoke(scoped);
                return (Func<T>) Func;
            };
            return factoryDependency;
        }
    }
}
