using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Singularity.Collections
{
    /// <summary>
    /// A thread safe and lock free dictionary.
    /// Uses <see cref="ImmutableAvlDictionary{TKey,TValue}"/> under the hood
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal class ThreadSafeDictionary<TKey, TValue> : IEnumerable<TValue>
        where TKey : class
    {
        public int Count => ImmutableAvlDictionary.Count;
        internal ImmutableHashTable<TKey, TValue> ImmutableAvlDictionary = ImmutableHashTable<TKey, TValue>.Empty;

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
                initialValue = ImmutableAvlDictionary;
                computedValue = ImmutableAvlDictionary.Add(key, value);
            }
            while (initialValue != Interlocked.CompareExchange(ref ImmutableAvlDictionary, computedValue, initialValue));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue Get(TKey key)
        {
            return ImmutableAvlDictionary.Get(key);
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return ImmutableAvlDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
