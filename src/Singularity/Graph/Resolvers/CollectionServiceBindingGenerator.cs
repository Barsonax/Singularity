using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Collections;
using Singularity.Expressions;

namespace Singularity.Graph.Resolvers
{
    /// <summary>
    /// Creates bindings for resolving all services of a given type.
    /// </summary>
    public sealed class CollectionServiceBindingGenerator : IServiceBindingGenerator
    {
        /// <inheritdoc />
        public IEnumerable<ServiceBinding> Resolve(IResolverPipeline graph, Type type)
        {
            Type? elementType = null;
            if (type.IsArray)
            {
                elementType = type.GetElementType();
            }
            else if (type.IsGenericType)
            {
                Type openGenericCollectionType = type.GetGenericTypeDefinition();

                Type[] collectionTypes =
                {
                    typeof(IEnumerable<>),
                    typeof(IReadOnlyCollection<>),
                    typeof(IReadOnlyList<>),

                    typeof(List<>),
                    typeof(ICollection<>),

                    typeof(IList<>),

                    typeof(HashSet<>),
                    typeof(ISet<>),
                };
                if (collectionTypes.Contains(openGenericCollectionType))
                {
                    elementType = type.GenericTypeArguments[0];

                }
            }

            if (elementType != null)
            {
                MethodInfo resolveMethod = typeof(CollectionServiceBindingGenerator).GetRuntimeMethods().Single(x => x.Name == nameof(Resolve) && x.ContainsGenericParameters).MakeGenericMethod(elementType);

                var bindings = (IEnumerable<ServiceBinding>)resolveMethod.Invoke(this, new object[] { graph, type });
                foreach (var binding in bindings)
                {
                    yield return binding;
                }
            }
        }

        private IEnumerable<ServiceBinding> Resolve<TElement>(IResolverPipeline graph, Type type)
        {
            Func<Scoped, object>[] instanceFactories = graph.TryResolveAll(typeof(TElement)).Select(x => x.Factory).ToArray();

            Expression expression = Expression.New(ConstructorResolvers.Default.StaticSelectConstructor(typeof(InstanceFactoryList<TElement>)), ExpressionGenerator.ScopeParameter, Expression.Constant(instanceFactories));

            yield return new ServiceBinding(new []
            {
                typeof(IEnumerable<TElement>),
                typeof(IReadOnlyCollection<TElement>),
                typeof(IReadOnlyList<TElement>),
            }, BindingMetadata.GeneratedInstance, expression, expression.GetReturnType(), ConstructorResolvers.Default, Lifetimes.Transient);

            //lists
            Expression<Func<Scoped, List<TElement>>> listExpression = scope => CreateList<TElement>(scope, instanceFactories);
            yield return new ServiceBinding(new[]
            {
                typeof(List<TElement>),
                typeof(ICollection<TElement>),
            }, BindingMetadata.GeneratedInstance, listExpression, expression.GetReturnType(), ConstructorResolvers.Default, Lifetimes.Transient);

            //sets
            Expression<Func<Scoped, HashSet<TElement>>> setExpression = scope => CreateSet<TElement>(scope, instanceFactories);
            yield return new ServiceBinding(new []
            {
                typeof(HashSet<TElement>),
                typeof(ISet<TElement>),
            }, BindingMetadata.GeneratedInstance, setExpression, expression.GetReturnType(), ConstructorResolvers.Default, Lifetimes.Transient);

            //arrays
            Expression<Func<Scoped, TElement[]>> arrayExpression = scope => CreateArray<TElement>(scope, instanceFactories);
            yield return new ServiceBinding(new []
            {
                typeof(TElement[]),
                typeof(IList<TElement>),
            }, BindingMetadata.GeneratedInstance, arrayExpression, expression.GetReturnType(), ConstructorResolvers.Default, Lifetimes.Transient);
        }

        private static List<TElement> CreateList<TElement>(Scoped scope, Func<Scoped, object>[] instanceFactories)
        {
            var list = new List<TElement>(instanceFactories.Length);

            foreach (Func<Scoped, object> instanceFactory in instanceFactories)
            {
                list.Add((TElement)instanceFactory.Invoke(scope));
            }

            return list;
        }

        private static HashSet<TElement> CreateSet<TElement>(Scoped scope, Func<Scoped, object>[] instanceFactories)
        {
            var list = new HashSet<TElement>();

            foreach (Func<Scoped, object> instanceFactory in instanceFactories)
            {
                list.Add((TElement)instanceFactory.Invoke(scope));
            }

            return list;
        }

        private static TElement[] CreateArray<TElement>(Scoped scope, Func<Scoped, object>[] instanceFactories)
        {
            var list = new TElement[instanceFactories.Length];

            for (int i = 0; i < instanceFactories.Length; i++)
            {
                list[i] = (TElement)instanceFactories[i].Invoke(scope);
            }

            return list;
        }
    }
}