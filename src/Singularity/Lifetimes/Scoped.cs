using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Singularity.Collections;

namespace Singularity
{
    /// <summary>
    /// Represents the scope in which instances will life.
    /// </summary>
    public sealed class Scoped : IContainer
    {
        internal static readonly MethodInfo AddDisposableMethod = typeof(Scoped).GetRuntimeMethods().Single(x => x.Name == nameof(AddDisposable));
        internal static readonly MethodInfo AddFinalizerMethod = typeof(Scoped).GetRuntimeMethods().Single(x => x.Name == nameof(AddFinalizer));
        internal static readonly MethodInfo GetOrAddScopedInstanceMethod = typeof(Scoped).GetRuntimeMethods().Single(x => x.Name == nameof(GetOrAddScopedInstance));
        private static readonly Action<IDisposable> DisposeAction = x => x.Dispose();

        private ThreadSafeDictionary<ServiceBinding, ActionList<object>> Finalizers { get; } = new ThreadSafeDictionary<ServiceBinding, ActionList<object>>();
        private ActionList<IDisposable> Disposables { get; } = new ActionList<IDisposable>(DisposeAction);
        private ThreadSafeDictionary<Type, object> ScopedInstances { get; } = new ThreadSafeDictionary<Type, object>();
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
            var instance = (T)ScopedInstances.GetOrDefault(key);
            if (instance != null) return instance;

            lock (ScopedInstances)
            {
                instance = (T)ScopedInstances.GetOrDefault(key);
                if (instance != null) return instance;
                instance = factory(this);
                ScopedInstances.Add(key, instance);

                return instance;
            }
        }

        internal T AddDisposable<T>(T obj)
            where T : class, IDisposable
        {
            Disposables.Add(obj);
            return obj;
        }

        internal T AddFinalizer<T>(T obj, ServiceBinding serviceBinding)
            where T : class
        {
            ActionList<object> list = Finalizers.GetOrDefault(serviceBinding);
            if (list != null)
            {
                list.Add(obj);
                return obj;
            }

            lock (Finalizers)
            {
                list = Finalizers.GetOrDefault(serviceBinding);
                if (list == null)
                {
                    list = new ActionList<object>(serviceBinding.Finalizer!);
                    Finalizers.Add(serviceBinding, list);
                }
            }
            list.Add(obj!);
            return obj;
        }

        /// <summary>
        /// Disposes the scope, calling all dispose actions on the instances that where created in this scope.
        /// </summary>
        public void Dispose()
        {
            Disposables.Invoke();

            if (Finalizers.Count > 0)
            {
                foreach (ActionList<object> objectActionList in Finalizers)
                {
                    objectActionList.Invoke();
                }
            }
        }
    }
}
