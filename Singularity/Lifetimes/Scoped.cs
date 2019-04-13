using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Singularity
{
    /// <summary>
    /// Represents the scope in which instances will life.
    /// </summary>
    public sealed class Scoped : IContainer
    {
        internal static readonly MethodInfo GenericAddMethod = typeof(Scoped).GetRuntimeMethods().FirstOrDefault(x => x.Name == nameof(AddDisposable));
        internal static readonly MethodInfo GenericGetorAddScopedInstanceMethod = typeof(Scoped).GetRuntimeMethods().FirstOrDefault(x => x.Name == nameof(GetorAddScopedInstance));
        private readonly object _locker = new object();
        private Dictionary<Binding, DisposeList> DisposeList { get; } = new Dictionary<Binding, DisposeList>();
        private Dictionary<Type, object> ScopedInstances { get; } = new Dictionary<Type, object>();

        internal Scoped() { }

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


        internal T GetorAddScopedInstance<T>(Type key, Func<Scoped, object> factory)
        {
            lock (ScopedInstances)
            {
                if (!ScopedInstances.TryGetValue(key, out object instance))
                {
                    instance = factory(this);
                    ScopedInstances.Add(key, instance);
                }

                return (T)instance;
            }
        }

        internal T AddDisposable<T>(T obj, Binding binding)
        {
            DisposeList list;
            lock (_locker)
            {
                if (!DisposeList.TryGetValue(binding, out list))
                {
                    list = new DisposeList(binding.OnDeathAction!);
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
            lock (_locker)
            {
                foreach (DisposeList objectActionList in DisposeList.Values)
                {
                    objectActionList.Invoke();
                }
            }
        }
    }
}
