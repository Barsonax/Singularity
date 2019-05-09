﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Singularity.Collections
{
    /// <summary>
    /// A thread safe and lock free dictionary.
    /// Uses <see cref="ImmutableHashTable{TKey,TValue}"/> under the hood
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal sealed class ThreadSafeDictionary<TKey, TValue> : IEnumerable<TValue>
        where TKey : class
    {
        public int Count => _immutableHashTable.Count;
        private ImmutableHashTable<TKey, TValue> _immutableHashTable = ImmutableHashTable<TKey, TValue>.Empty;

        /// <summary>
        /// Adds a key to the dictionary.
        /// Thread safe.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            ImmutableHashTable<TKey, TValue> initialValue, computedValue;
            do
            {
                initialValue = _immutableHashTable;
                computedValue = initialValue.Add(key, value);
            }
            while (initialValue != Interlocked.CompareExchange(ref _immutableHashTable, computedValue, initialValue));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue GetOrDefault(TKey key)
        {
            return _immutableHashTable.GetOrDefault(key);
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _immutableHashTable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
