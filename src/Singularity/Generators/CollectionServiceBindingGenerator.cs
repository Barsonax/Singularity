using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Bindings;
using Singularity.Resolvers;

namespace Singularity.Generators
{
    /// <summary>
    /// Creates bindings for resolving all services of a given type.
    /// </summary>
    public sealed class CollectionServiceBindingGenerator : IServiceBindingGenerator
    {
        private static readonly MethodInfo GenericResolveMethod = typeof(CollectionServiceBindingGenerator).GetRuntimeMethods().Single(x => x.Name == nameof(TryGenerate) && x.ContainsGenericParameters);

        /// <inheritdoc />
        public IEnumerable<ServiceBinding> TryGenerate(IContainerContext context, Type type)
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
                MethodInfo resolveMethod = GenericResolveMethod.MakeGenericMethod(elementType);

                var bindings = (IEnumerable<ServiceBinding>)resolveMethod.Invoke(this, new object[] { context, type });
                foreach (var binding in bindings)
                {
                    yield return binding;
                }
            }
        }

        private IEnumerable<ServiceBinding> TryGenerate<TElement>(IContainerContext graph, Type type)
        {
            Func<Scoped, TElement>[] instanceFactories = graph.TryResolveAll(typeof(TElement)).Select(x => (Func<Scoped, TElement>)(Delegate)x.Factory).ToArray();

            yield return new ServiceBinding(new[]
            {
                typeof(Func<Scoped, TElement>[]),
            }, BindingMetadata.GeneratedInstance, Expression.Constant(instanceFactories), typeof(Func<Scoped, TElement>[]), ConstructorResolvers.Default, Lifetimes.PerContainer);

            Expression expression = ConstructorResolvers.Default.ResolveConstructorExpression(typeof(InstanceFactoryList<TElement>))!;

            yield return new ServiceBinding(new[]
            {
                typeof(IEnumerable<TElement>),
                typeof(IReadOnlyCollection<TElement>),
                typeof(IReadOnlyList<TElement>),
            }, BindingMetadata.GeneratedInstance, expression, expression.GetReturnType(), ConstructorResolvers.Default, Lifetimes.Transient);

            //lists
            Expression<Func<Scoped, List<TElement>>> listExpression = scope => CreateList(scope, instanceFactories);
            yield return new ServiceBinding(new[]
            {
                typeof(List<TElement>),
                typeof(ICollection<TElement>),
            }, BindingMetadata.GeneratedInstance, listExpression, listExpression.GetReturnType(), ConstructorResolvers.Default, Lifetimes.Transient);

            //sets
            Expression<Func<Scoped, HashSet<TElement>>> setExpression = scope => CreateSet(scope, instanceFactories);
            yield return new ServiceBinding(new[]
            {
                typeof(HashSet<TElement>),
                typeof(ISet<TElement>),
            }, BindingMetadata.GeneratedInstance, setExpression, setExpression.GetReturnType(), ConstructorResolvers.Default, Lifetimes.Transient);

            //arrays
            Expression<Func<Scoped, TElement[]>> arrayExpression = scope => CreateArray(scope, instanceFactories);
            yield return new ServiceBinding(new[]
            {
                typeof(TElement[]),
                typeof(IList<TElement>),
            }, BindingMetadata.GeneratedInstance, arrayExpression, arrayExpression.GetReturnType(), ConstructorResolvers.Default, Lifetimes.Transient);
        }

        private static List<TElement> CreateList<TElement>(Scoped scope, Func<Scoped, TElement>[] instanceFactories)
        {
            var list = new List<TElement>(instanceFactories.Length);

            foreach (Func<Scoped, TElement> instanceFactory in instanceFactories)
            {
                list.Add(instanceFactory.Invoke(scope));
            }

            return list;
        }

        private static HashSet<TElement> CreateSet<TElement>(Scoped scope, Func<Scoped, TElement>[] instanceFactories)
        {
            var list = new HashSet<TElement>();

            foreach (Func<Scoped, TElement> instanceFactory in instanceFactories)
            {
                list.Add(instanceFactory.Invoke(scope));
            }

            return list;
        }

        private static TElement[] CreateArray<TElement>(Scoped scope, Func<Scoped, TElement>[] instanceFactories)
        {
            var list = new TElement[instanceFactories.Length];

            for (int i = 0; i < instanceFactories.Length; i++)
            {
                list[i] = instanceFactories[i].Invoke(scope);
            }

            return list;
        }

        private sealed class InstanceFactoryList<T> : IReadOnlyList<T>
        {
            private readonly Func<Scoped, T>[] _instanceFactories;
            private readonly Scoped _scope;

            public InstanceFactoryList(Scoped scope, Func<Scoped, T>[] instanceFactories)
            {
                _instanceFactories = instanceFactories;
                _scope = scope;
            }

            public IEnumerator<T> GetEnumerator()
            {
                foreach (Func<Scoped, T> instanceFactory in _instanceFactories)
                {
                    yield return instanceFactory(_scope);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public int Count => _instanceFactories.Length;

            public T this[int index] => _instanceFactories[index](_scope);
        }
    }
}