using System;
using System.Collections.Generic;
using Singularity.Bindings;
using Singularity.Graph;

namespace Singularity
{
    public class ThreadSafeContainer
    {
        private readonly Container _container;
        private readonly object _locker;

        internal ThreadSafeContainer(Container container, object? locker = null)
        {
            _container = container;
            _locker = locker ?? new object();
        }

        public object GetInstance(Type type)
        {
            lock (_locker)
            {
                return _container.GetInstance(type);
            }
        }

        public T GetInstance<T>() where T : class
        {
            lock (_locker)
            {
                return _container.GetInstance<T>();
            }
        }

        public Func<object> GetInstanceFactory(Type type)
        {
            lock (_locker)
            {
                return _container.GetInstanceFactory(type);
            }
        }

        public Func<T> GetInstanceFactory<T>() where T : class
        {
            lock (_locker)
            {
                return _container.GetInstanceFactory<T>();
            }
        }

        public Action<object> GetMethodInjector(Type type)
        {
            lock (_locker)
            {
                return _container.GetMethodInjector(type);
            }
        }

        public ThreadSafeContainer GetNestedContainer(IEnumerable<Binding> bindings)
        {
            var container = _container.GetNestedContainer(bindings);
            return new ThreadSafeContainer(container, _locker);
        }

        public ThreadSafeContainer GetNestedContainer(IEnumerable<IModule> modules)
        {
            var container = _container.GetNestedContainer(modules);
            return new ThreadSafeContainer(container, _locker);
        }

        public void MethodInject(object instance)
        {
            lock (_locker)
            {
                _container.MethodInject(instance);
            }
        }

        public void MethodInjectAll<T>(IEnumerable<T> instances)
        {
            lock (_locker)
            {
                _container.MethodInjectAll(instances);
            }
        }
    }
}
