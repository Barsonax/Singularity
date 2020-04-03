using System;
using System.Collections;
using System.Collections.Generic;

namespace Singularity.Collections
{
    internal sealed class InstanceFactoryList<T> : IReadOnlyList<T>
    {
        private readonly Func<Scoped, T>[] _instanceFactories;
        private readonly Scoped _scope;

        public InstanceFactoryList(Scoped scope, Func<Scoped, T>[] instanceFactories)
        {
            _instanceFactories = instanceFactories ?? throw new ArgumentNullException(nameof(instanceFactories));
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