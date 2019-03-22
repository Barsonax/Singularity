using System;
using System.Runtime.CompilerServices;

namespace Singularity.Collections
{
    internal sealed class ImmutableDictionary<TKey, TValue>
    {
        public static readonly ImmutableDictionary<TKey, TValue> Empty = new ImmutableDictionary<TKey, TValue>();
        public readonly int Count;
        public readonly ImmutableAvlNode<TKey, TValue>[] Buckets;
        public readonly int Divisor;

        public ImmutableDictionary<TKey, TValue> Add(TKey key, TValue value)
        {
            return new ImmutableDictionary<TKey, TValue>(this, new KeyValue<TKey, TValue>(key, value));
        }

        public TValue this[TKey key] => Search(key);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue Search(TKey key)
        {
            var hashCode = RuntimeHelpers.GetHashCode(key);
            var bucketIndex = hashCode & (Divisor - 1);
            ImmutableAvlNode<TKey, TValue> avlNode = Buckets[bucketIndex];

            while (avlNode.Height != 0 && avlNode.KeyValue.HashCode != hashCode)
            {
                avlNode = hashCode < avlNode.KeyValue.HashCode ? avlNode.Left! : avlNode.Right!;
            }

            if (avlNode.Height != 0 && (ReferenceEquals(avlNode.KeyValue.Key, key) || Equals(avlNode.KeyValue.Key, key)))
            {
                return avlNode.KeyValue.Value;
            }

            if (avlNode.Duplicates.Items.Length > 0)
            {
                foreach (var keyValue in avlNode.Duplicates.Items)
                {
                    if (ReferenceEquals(keyValue.Key, key) || Equals(keyValue.Key, key))
                    {
                        return keyValue.Value;
                    }
                }
            }

            return default!;
        }

        private ImmutableDictionary(ImmutableDictionary<TKey, TValue> previous, KeyValue<TKey, TValue> keyValue)
        {
            this.Count = previous.Count + 1;
            if (previous.Count >= previous.Divisor)
            {
                this.Divisor = previous.Divisor * 2;
                this.Buckets = new ImmutableAvlNode<TKey, TValue>[this.Divisor];
                InitializeBuckets(0, this.Divisor);
                this.AddExistingValues(previous);
            }
            else
            {
                this.Divisor = previous.Divisor;
                this.Buckets = new ImmutableAvlNode<TKey, TValue>[this.Divisor];
                Array.Copy(previous.Buckets, this.Buckets, previous.Divisor);
            }

            var bucketIndex = keyValue.HashCode & (this.Divisor - 1);
            this.Buckets[bucketIndex] = this.Buckets[bucketIndex].Add(keyValue);
        }

        private ImmutableDictionary()
        {
            this.Buckets = new ImmutableAvlNode<TKey, TValue>[2];
            this.Divisor = 2;
            InitializeBuckets(0, 2);
        }

        private void AddExistingValues(ImmutableDictionary<TKey, TValue> previous)
        {
            foreach (ImmutableAvlNode<TKey, TValue> bucket in previous.Buckets)
            {
                foreach (var keyValue in bucket.InOrder())
                {
                    int hashCode = RuntimeHelpers.GetHashCode(keyValue.Key);
                    int bucketIndex = hashCode & (this.Divisor - 1);
                    this.Buckets[bucketIndex] = this.Buckets[bucketIndex].Add(keyValue);
                }
            }
        }

        private void InitializeBuckets(int startIndex, int count)
        {
            for (int i = startIndex; i < count; i++)
            {
                this.Buckets[i] = ImmutableAvlNode<TKey, TValue>.Empty;
            }
        }
    }
}
