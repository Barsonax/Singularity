using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Singularity.Collections;

namespace Singularity
{
    /// <summary>
    /// A light weight scope in which instances will life.
    /// </summary>
    public sealed class Scoped : IContainer
    {
        internal static readonly MethodInfo AddDisposableMethod = typeof(Scoped).GetRuntimeMethods().Single(x => x.Name == nameof(AddDisposable));
        internal static readonly MethodInfo AddFinalizerMethod = typeof(Scoped).GetRuntimeMethods().Single(x => x.Name == nameof(AddFinalizer));
        internal static readonly MethodInfo GetOrAddScopedInstanceMethod = typeof(Scoped).GetRuntimeMethods().Single(x => x.Name == nameof(GetOrAddScopedInstance));

        private SinglyLinkedListKeyNode<ServiceBinding, ActionList<object>> _finalizers;
        private SinglyLinkedListNode<IDisposable> _disposables;
        private SinglyLinkedListKeyNode<Type, object> _scopedInstances;
        private readonly Container _container;

        internal Scoped(Container container)
        {
            _container = container;
        }

        /// <summary>
        /// <see cref="Singularity.Container.GetInstance{T}()"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetInstance<T>() where T : class
        {
            return (T)GetInstance(typeof(T));
        }

        /// <summary>
        /// <see cref="Singularity.Container.GetInstance(Type)"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetInstance(Type type)
        {
            return _container.GetInstance(type, this);
        }

        /// <summary>
        /// <see cref="Container.LateInject(object)"/>
        /// </summary>
        /// <param name="instance"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LateInject(object instance)
        {
            _container.LateInject(instance, this);
        }

        /// <summary>
        /// <see cref="Container.LateInjectAll{T}(IEnumerable{T})"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instances"></param>
        public void LateInjectAll<T>(IEnumerable<T> instances)
        {
            _container.LateInjectAll(instances, this);
        }

        internal T GetOrAddScopedInstance<T>(Func<Scoped, T> factory, Type key)
            where T : class
        {
            SinglyLinkedListKeyNode<Type, object>? initialValue, computedValue;
            do
            {
                initialValue = _scopedInstances;
                var instance = (T)initialValue.GetOrDefault(key);
                if (instance != null) return instance;
                T obj = factory(this); //There is a very slight chance that this instance is created more than once under heavy load.
                computedValue = initialValue.Add(key, obj);
            }
            while (initialValue != Interlocked.CompareExchange(ref _scopedInstances, computedValue, initialValue));

            return (T)computedValue.Value;
        }

        internal T AddDisposable<T>(T obj)
            where T : class, IDisposable
        {
            SinglyLinkedListNode<IDisposable>? initialValue, computedValue;
            do
            {
                initialValue = _disposables;
                computedValue = initialValue.Add(obj);
            }
            while (initialValue != Interlocked.CompareExchange(ref _disposables, computedValue, initialValue));
            return obj;
        }

        internal T AddFinalizer<T>(T obj, ServiceBinding key)
            where T : class
        {
            SinglyLinkedListKeyNode<ServiceBinding, ActionList<object>>? initialValue, computedValue;
            do
            {
                initialValue = _finalizers;
                ActionList<object> list = initialValue.GetOrDefault(key);
                if (list != null)
                {
                    list.Add(obj);
                    return obj;
                }

                computedValue = initialValue.Add(key, new ActionList<object>(key.Finalizer!));
            }
            while (initialValue != Interlocked.CompareExchange(ref _finalizers, computedValue, initialValue));
            computedValue.Value.Add(obj);
            return obj;
        }

        /// <summary>
        /// Disposes the scope, calling all dispose actions on the instances that where created in this scope.
        /// </summary>
        public void Dispose()
        {
            SinglyLinkedListNode<IDisposable>? disposables = _disposables;
            while (disposables != null)
            {
                disposables.Value.Dispose();
                disposables = disposables.Next!;
            }

            SinglyLinkedListKeyNode<ServiceBinding, ActionList<object>>? finalizers = _finalizers;
            while (finalizers != null)
            {
                finalizers.Value.Invoke();
                finalizers = finalizers.Next;
            }
        }
    }
}
