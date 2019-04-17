using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Singularity.Collections;

namespace Singularity
{
    /// <summary>
    /// Represents the scope in which instances will life.
    /// </summary>
    public sealed class Scoped : IContainer
    {
        internal static readonly MethodInfo AddMethod = typeof(Scoped).GetRuntimeMethods().FirstOrDefault(x => x.Name == nameof(AddDisposable));
        internal static readonly MethodInfo GetOrAddScopedInstanceMethod = typeof(Scoped).GetRuntimeMethods().FirstOrDefault(x => x.Name == nameof(GetOrAddScopedInstance));
        private Dictionary<Binding, DisposeList<object>> DisposeList { get; } = new Dictionary<Binding, DisposeList<object>>();
        private ThreadSafeDictionary<Type, object> ScopedInstances { get; } = new ThreadSafeDictionary<Type, object>();
        public readonly Container Container;

        internal Scoped(Container container)
        {
            Container = container;
        }

        /// <summary>
        /// <see cref="Singularity.Container.GetInstance{T}()"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetInstance<T>() where T : class
        {
            return (T)GetInstance(typeof(T));
        }

        /// <summary>
        /// <see cref="Singularity.Container.GetInstance(Type)"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object GetInstance(Type type)
        {
            return Container.GetInstance(type, this);
        }

        /// <summary>
        /// <see cref="Singularity.Container.MethodInject(object)"/>
        /// </summary>
        /// <param name="instance"></param>
        public void MethodInject(object instance)
        {
            Container.MethodInject(instance, this);
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

        internal T AddDisposable<T>(T obj, Binding binding)
        {
            DisposeList<object> list;
            lock (DisposeList)
            {
                if (!DisposeList.TryGetValue(binding, out list))
                {
                    list = new DisposeList<object>(binding.OnDeathAction!);
                    DisposeList.Add(binding, list);
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
            lock (DisposeList)
            {
                foreach (DisposeList<object> objectActionList in DisposeList.Values)
                {
                    objectActionList.Invoke();
                }
            }
        }
    }
}
