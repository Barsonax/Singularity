using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Singularity.Collections;

namespace Singularity.Resolving.Generators
{
    /// <summary>
    /// Creates bindings for resolving all services of a given type.
    /// </summary>
    public sealed class CollectionServiceBindingGenerator : IServiceBindingGenerator, IGenericWrapperGenerator
    {
        private static readonly MethodInfo GenericResolveMethod = typeof(CollectionServiceBindingGenerator).GetRuntimeMethods().Single(x => x.Name == nameof(Resolve) && x.ContainsGenericParameters);

        /// <inheritdoc />
        public IEnumerable<ServiceBinding> Resolve(IInstanceFactoryResolver resolver, Type type)
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
                    typeof(IList<>),
                    typeof(ICollection<>),

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

                var bindings = (IEnumerable<ServiceBinding>)resolveMethod.Invoke(this, new object[] { resolver, type });
                foreach (var binding in bindings)
                {
                    yield return binding;
                }
            }
        }

        private IEnumerable<ServiceBinding> Resolve<TElement>(IInstanceFactoryResolver resolver, Type type)
        {
            Func<Scoped, TElement>[] instanceFactories = resolver.FindApplicableBindings(typeof(TElement))
                .Select(x => resolver.TryResolveDependency(typeof(TElement), x))
                .Where(x => x != null)
                .Select(x => (Func<Scoped, TElement>)((Scoped scoped) => (TElement)x!.Factory(scoped)!))
                .ToArray();

            yield return new ServiceBinding(new[]
            {
                typeof(Func<Scoped, TElement>[]),
            }, BindingMetadata.GeneratedInstance, Expression.Constant(instanceFactories), typeof(Func<Scoped, TElement>[]), ConstructorResolvers.Default, Lifetimes.PerContainer);

            //lists
            Expression<Func<Scoped, List<TElement>>> listExpression = scope => CreateList(scope, instanceFactories);
            yield return new ServiceBinding(new[]
            {
                typeof(List<TElement>),
                typeof(IList<TElement>),
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
            }, BindingMetadata.GeneratedInstance, arrayExpression, arrayExpression.GetReturnType(), ConstructorResolvers.Default, Lifetimes.Transient);

            //enumerable
            Expression expression = ConstructorResolvers.Default.ResolveConstructorExpression(typeof(InstanceFactoryList<TElement>));

            yield return new ServiceBinding(new[]
            {
                typeof(IEnumerable<TElement>),
                typeof(IReadOnlyCollection<TElement>),
                typeof(IReadOnlyList<TElement>),
            }, BindingMetadata.GeneratedInstance, expression, expression.GetReturnType(), ConstructorResolvers.Default, Lifetimes.Transient);
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

        public bool CanResolve(Type type) => type.IsGenericType && new[] { typeof(IEnumerable<>), typeof(IReadOnlyCollection<>), typeof(IReadOnlyList<>), }.Contains(type.GetGenericTypeDefinition());

        public Expression Wrap(IInstanceFactoryResolver resolver, Expression expression, Type unWrappedType, Type wrappedType)
        {
            var elementType = wrappedType.GetGenericArguments()[0];
            var ex = ConstructorResolvers.Default.ResolveConstructorExpression(typeof(InstanceFactoryList<>).MakeGenericType(elementType));

            return ex;
        }

        public Type? DependsOn(Type type) => typeof(Func<,>).MakeGenericType(typeof(Scoped), type).MakeArrayType();
    }

    public class ScopedFuncGenerator : IGenericWrapperGenerator
    {
        private static readonly MethodInfo GenericResolveMethod = typeof(ScopedFuncGenerator).GetRuntimeMethods().Single(x => x.Name == nameof(Resolve) && x.ContainsGenericParameters);

        public bool CanResolve(Type type)
        {
            return type.IsArray && type.GetElementType().IsGenericType && type.GetElementType().GetGenericTypeDefinition() == typeof(Func<,>) && type.GetElementType().GetGenericArguments()[0] == typeof(Scoped);
        }

        public Type? DependsOn(Type type)
        {
            return null;
        }

        public Expression Wrap(IInstanceFactoryResolver resolver, Expression expression, Type unWrappedType, Type wrappedType)
        {
            var elementType = wrappedType.GetGenericArguments()[1];
            MethodInfo resolveMethod = GenericResolveMethod.MakeGenericMethod(elementType);
            return (Expression)resolveMethod.Invoke(this, new object[] { resolver });
        }

        public Expression Resolve<TElement>(IInstanceFactoryResolver resolver)
        {
            Func<Scoped, TElement>[] instanceFactories = resolver.FindApplicableBindings(typeof(TElement))
                .Select(x => resolver.TryResolveDependency(typeof(TElement), x))
                .Where(x => x != null)
                .Select(x => (Func<Scoped, TElement>)((Scoped scoped) => (TElement)x!.Factory(scoped)!))
                .ToArray();
            return Expression.Constant(instanceFactories);
        }
    }
}