using System.Runtime.CompilerServices;
using System.Threading;

namespace Singularity.Collections
{
    /// <summary>
    /// A thread safe and lock free dictionary.
    /// Uses <see cref="ImmutableDictionary{TKey,TValue}"/> under the hood
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal class ThreadSafeDictionary<TKey, TValue>
        where TKey : class
    {
        public int Count => _immutableDictionary.Count;
        internal ImmutableDictionary<TKey, TValue> _immutableDictionary = ImmutableDictionary<TKey, TValue>.Empty;

        /// <summary>
        /// Adds a key to the dictionary.
        /// Thread safe.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            ImmutableDictionary<TKey, TValue> initialValue, computedValue;
            do
            {
                initialValue = _immutableDictionary;
                computedValue = _immutableDictionary.Add(key, value);
            }
            while (initialValue != Interlocked.CompareExchange(ref _immutableDictionary, computedValue, initialValue));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue Search(TKey key)
        {
            return _immutableDictionary.Search(key);
        }
    }
}
