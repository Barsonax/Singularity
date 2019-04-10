using System;
using System.Collections;
using System.Collections.Generic;

namespace Singularity.Collections
{
    internal class InstanceFactoryList<T> : IReadOnlyList<T>
    {
        private readonly Func<Scoped, object>[] _instanceFactories;
        private readonly Scoped _scope;

        public InstanceFactoryList(Scoped scope, Func<Scoped, object>[] instanceFactories)
        {
            _instanceFactories = instanceFactories ?? throw new ArgumentNullException(nameof(instanceFactories));
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (Func<Scoped, object> instanceFactory in _instanceFactories)
            {
                yield return (T)instanceFactory.Invoke(_scope);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _instanceFactories.Length;

        public T this[int index] => (T)_instanceFactories[index].Invoke(_scope);
    }
}