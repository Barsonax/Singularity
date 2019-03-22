using System;
using System.Runtime.CompilerServices;

namespace Singularity.Collections
{
    internal sealed class ImmutableDictionary<TKey, TValue>
    {
        public static readonly ImmutableDictionary<TKey, TValue> Empty = new ImmutableDictionary<TKey, TValue>();
        public readonly int Count;
        internal readonly ImmutableAvlNode<TKey, TValue>[] Buckets;
        internal readonly int Divisor;

        public ImmutableDictionary<TKey, TValue> Add(TKey key, TValue value)
        {
            return new ImmutableDictionary<TKey, TValue>(this, key, value);
        }

        public TValue this[TKey key] => Search(key);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue Search(TKey key)
        {
            var hashCode = RuntimeHelpers.GetHashCode(key);
            var bucketIndex = hashCode & (Divisor - 1);
            ImmutableAvlNode<TKey, TValue> avlNode = Buckets[bucketIndex];

            while (avlNode.Height != 0 && avlNode.HashCode != hashCode)
            {
                avlNode = hashCode < avlNode.HashCode ? avlNode.Left : avlNode.Right;
            }

            if (avlNode.Height != 0 && (ReferenceEquals(avlNode.Key, key) || Equals(avlNode.Key, key)))
            {
                return avlNode.Value;
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

            return default;
        }

        private ImmutableDictionary(ImmutableDictionary<TKey, TValue> previous, TKey key, TValue value)
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

            var hashCode = RuntimeHelpers.GetHashCode(key);
            var bucketIndex = hashCode & (this.Divisor - 1);
            this.Buckets[bucketIndex] = this.Buckets[bucketIndex].Add(key, value);
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
                    this.Buckets[bucketIndex] = this.Buckets[bucketIndex].Add(keyValue.Key, keyValue.Value);
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
