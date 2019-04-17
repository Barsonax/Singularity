using System;
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
        internal static readonly Action<IDisposable> DisposeAction = x => x.Dispose();

        private ThreadSafeDictionary<Binding, ActionList<object>> Finalizers { get; } = new ThreadSafeDictionary<Binding, ActionList<object>>();
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
        /// <see cref="Singularity.Container.MethodInject(object)"/>
        /// </summary>
        /// <param name="instance"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MethodInject(object instance)
        {
            _container.MethodInject(instance, this);
        }

        internal T GetOrAddScopedInstance<T>(Func<Scoped, T> factory, Type key)
        {
            T instance = (T)ScopedInstances.Search(key);
            if (instance != null) return instance;

            lock (ScopedInstances)
            {
                instance = (T)ScopedInstances.Search(key);
                if (instance != null) return instance;
                instance = factory(this);
                ScopedInstances.Add(key, instance);

                return instance;
            }
        }

        internal T AddDisposable<T>(T obj)
            where T : IDisposable
        {
            Disposables.Add(obj);
            return obj;
        }

        internal T AddFinalizer<T>(T obj, Binding binding)
        {
            ActionList<object> list = Finalizers.Search(binding);
            if (list != null)
            {
                list.Add(obj);
                return obj;
            }

            lock (Finalizers)
            {
                list = Finalizers.Search(binding);
                if (list == null)
                {
                    list = new ActionList<object>(binding.Finalizer);
                    Finalizers.Add(binding, list);
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
                foreach (ActionList<object> objectActionList in Finalizers)
                {
                    objectActionList.Invoke();
                }
        }
    }
}
