using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Singularity.Collections
{
    internal sealed class ImmutableHashTable<TKey, TValue> : IEnumerable<TValue>
        where TKey : class
    {
        public static readonly ImmutableHashTable<TKey, TValue> Empty = new ImmutableHashTable<TKey, TValue>();
        public readonly int Count;
        private readonly SinglyLinkedListNode<HashedKeyValue<TKey, TValue>>[] Buckets;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableHashTable<TKey, TValue> Add(TKey key, TValue value)
        {
            return new ImmutableHashTable<TKey, TValue>(this, new HashedKeyValue<TKey, TValue>(key, value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal TValue Get(TKey key)
        {
            int hashCode = HashHelpers.GetHashCode(key);
            int bucketIndex = hashCode & (Buckets.Length - 1);

            SinglyLinkedListNode<HashedKeyValue<TKey, TValue>>? current = Buckets[bucketIndex];

            while (current != null)
            {
                if (ReferenceEquals(current.Value.Key, key)) return current.Value.Value;
                current = current.Next;
            }

            return default!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ImmutableHashTable(ImmutableHashTable<TKey, TValue> previous, in HashedKeyValue<TKey, TValue> hashedKeyValue)
        {
            Count = previous.Count + 1;
            if (previous.Count >= previous.Buckets.Length)
            {
                Buckets = new SinglyLinkedListNode<HashedKeyValue<TKey, TValue>>[previous.Buckets.Length * 2];
                foreach (SinglyLinkedListNode<HashedKeyValue<TKey, TValue>> t in previous.Buckets)
                {
                    SinglyLinkedListNode<HashedKeyValue<TKey, TValue>>? current = t;
                    while (current != null)
                    {
                        FillBucket(in current.Value);
                        current = current.Next;
                    }
                }
            }
            else
            {
                Buckets = new SinglyLinkedListNode<HashedKeyValue<TKey, TValue>>[previous.Buckets.Length];
                for (var i = 0; i < previous.Buckets.Length; i++)
                {
                    Buckets[i] = previous.Buckets[i];
                }
            }

            FillBucket(in hashedKeyValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FillBucket(in HashedKeyValue<TKey, TValue> hashedKeyValue)
        {
            int bucketIndex = hashedKeyValue.HashCode & (Buckets.Length - 1);
            Buckets[bucketIndex] = Buckets[bucketIndex].Add(in hashedKeyValue);
        }

        private ImmutableHashTable()
        {
            Buckets = new SinglyLinkedListNode<HashedKeyValue<TKey, TValue>>[2];
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (SinglyLinkedListNode<HashedKeyValue<TKey, TValue>> bucket in Buckets)
            {
                SinglyLinkedListNode<HashedKeyValue<TKey, TValue>>? current = bucket;
                while (current != null)
                {
                    yield return current.Value.Value;
                    current = current.Next;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
