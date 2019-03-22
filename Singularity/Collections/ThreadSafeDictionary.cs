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
        where TValue : class
    {
        private ImmutableDictionary<TKey, TValue> _immutableDictionary = ImmutableDictionary<TKey, TValue>.Empty;

        /// <summary>
        /// Gets a key from the dictionary.
        /// Thread safe.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key] => _immutableDictionary[key];

        /// <summary>
        /// Adds a key to the dictionary.
        /// Thread safe.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            while (true)
            {
                if (Interlocked.CompareExchange(ref _immutableDictionary, _immutableDictionary.Add(key, value), _immutableDictionary) != _immutableDictionary)
                {
                    break;
                }
            }
        }
    }
}
