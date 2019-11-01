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

        private SinglyLinkedListKeyNode<ServiceBinding, ActionList<object>>? _finalizers;
        private SinglyLinkedListNode<IDisposable>? _disposables;
        private SinglyLinkedListKeyNode<Type, object>? _scopedInstances;
        internal readonly Container Container;

        internal Scoped(Container container)
        {
            Container = container;
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
            return Container.GetInstance(type, this);
        }

        /// <summary>
        /// <see cref="Singularity.Container.LateInject(object)"/>
        /// </summary>
        /// <param name="instance"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LateInject(object instance)
        {
            Container.LateInject(instance, this);
        }

        /// <inheritdoc />
        public T? GetInstanceOrDefault<T>() where T : class
        {
            return Container.GetInstanceOrDefault<T>();
        }

        /// <inheritdoc />
        public object? GetInstanceOrDefault(Type type)
        {
            return Container.GetInstanceOrDefault(type, this);
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            return Container.GetInstance(serviceType, this);
        }

        /// <summary>
        /// <see cref="Singularity.Container.LateInjectAll{T}(IEnumerable{T})"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instances"></param>
        public void LateInjectAll<T>(IEnumerable<T> instances)
        {
            Container.LateInjectAll(instances, this);
        }

        internal T GetOrAddScopedInstance<T>(Func<Scoped, T> factory, Type key)
        {
            SinglyLinkedListKeyNode<Type, object>? initialValue = _scopedInstances;
            SinglyLinkedListKeyNode<Type, object>? current = initialValue;
            while (current != null)
            {
                if (ReferenceEquals(current.Key, key)) return (T)current.Value;
                current = current.Next!;
            }

            //There is a very slight chance that this instance is created more than once under heavy load.
            //In that case the duplicate will be discarded.
            T obj = factory(this);
            SinglyLinkedListKeyNode<Type, object> computedValue = initialValue.Add(key, obj!);
            if (ReferenceEquals(Interlocked.CompareExchange(ref _scopedInstances, computedValue, initialValue), initialValue))
            {
                return obj;
            }
            else
            {
                return HandleScopeThreadCollision(obj, key);
            }
        }

        /// <summary>
        /// Internal accessor is for testing only.
        /// Handles the rare case where another thread has already modified <see cref="_scopedInstances"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        internal T HandleScopeThreadCollision<T>(T obj, Type key)
        {
            SinglyLinkedListKeyNode<Type, object>? initialValue, computedValue;
            do
            {
                initialValue = _scopedInstances;
                SinglyLinkedListKeyNode<Type, object>? current = initialValue;
                while (current != null)
                {
                    if (ReferenceEquals(current.Key, key)) return (T)current.Value;
                    current = current.Next!;
                }
                computedValue = initialValue.Add(key, obj!);
            }
            while (!ReferenceEquals(Interlocked.CompareExchange(ref _scopedInstances, computedValue, initialValue), initialValue));

            return obj;
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
            while (!ReferenceEquals(Interlocked.CompareExchange(ref _disposables, computedValue, initialValue), initialValue));
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
            while (!ReferenceEquals(Interlocked.CompareExchange(ref _finalizers, computedValue, initialValue), initialValue));
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
