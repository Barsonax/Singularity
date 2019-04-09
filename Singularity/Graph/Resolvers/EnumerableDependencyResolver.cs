using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Expressions;

namespace Singularity.Graph.Resolvers
{
    internal class EnumerableDependencyResolver : IDependencyResolver
    {
        private static readonly MethodInfo GenericCreateEnumerableMethod;
        private static readonly MethodInfo GenericCreateEnumerableDependencyMethod;

        static EnumerableDependencyResolver()
        {
            GenericCreateEnumerableMethod = (from m in typeof(EnumerableDependencyResolver).GetRuntimeMethods()
                where m.Name == nameof(CreateEnumerable)
                select m).Single();

            GenericCreateEnumerableDependencyMethod = (from m in typeof(EnumerableDependencyResolver).GetRuntimeMethods()
                where m.Name == nameof(CreateEnumerableDependency)
                select m).Single();
        }

        public Dependency? Resolve(DependencyGraph graph, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var childDependency = graph.TryGetDependency(type.GenericTypeArguments[0]);
                var dependencies = childDependency?.ResolvedDependencies.Array ?? new ResolvedDependency[0];
                foreach (ResolvedDependency dependency in dependencies)
                {
                    graph.ResolveDependency(dependency);
                }

                Func<Scoped, object>[] instanceFactories = dependencies.Select(x => x.InstanceFactory!).ToArray();

                var method = GenericCreateEnumerableDependencyMethod.MakeGenericMethod(type.GenericTypeArguments);
                Dependency enumerableDependency = (Dependency)method.Invoke(null, new object[] { type, instanceFactories });
                enumerableDependency.Default.Children = childDependency != null ? new[] { childDependency } : new Dependency[0];
                return enumerableDependency;
            }

            return null;
        }

        private static Dependency CreateEnumerableDependency<T>(Type type, Func<Scoped, object>[] instanceFactories)
        {
            MethodInfo createEnumerableMethod = GenericCreateEnumerableMethod.MakeGenericMethod(type.GenericTypeArguments);
            Expression expression = Expression.Call(createEnumerableMethod, ExpressionGenerator.ScopeParameter, Expression.Constant(instanceFactories));
            Dependency dependency = new Dependency(type, expression, CreationMode.Transient);
            dependency.Default.Expression = expression;
            dependency.Default.InstanceFactory = scoped => CreateEnumerable<T>(scoped, instanceFactories);
            return dependency;
        }

        private static IEnumerable<T> CreateEnumerable<T>(Scoped scope, Func<Scoped, object>[] instanceFactories)
        {
            foreach (Func<Scoped, object> instanceFactory in instanceFactories)
            {
                yield return (T)instanceFactory.Invoke(scope);
            }
        }
    }
}