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
    }

    internal static class ThreadSafeDictionaryReferenceTypes
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue Search<TKey, TValue, TKey2>(this ThreadSafeDictionary<TKey, TValue> instance, TKey2 key)
            where TKey2 : class, TKey
        {
            var hashCode = RuntimeHelpers.GetHashCode(key);
            return instance._immutableDictionary.SearchInternal(key, hashCode);
        }
    }

    internal static class ThreadSafeDictionaryValueTypes
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue Search<TKey, TValue, TKey2>(this ThreadSafeDictionary<TKey, TValue> instance, TKey2 key)
            where TKey2 : struct, TKey
        {
            var hashCode = key.GetHashCode();
            return instance._immutableDictionary.SearchInternal(key, hashCode);
        }
    }
}
