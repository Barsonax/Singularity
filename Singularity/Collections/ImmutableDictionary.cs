using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Singularity.Collections
{
    internal sealed class ImmutableDictionary<TKey, TValue> : IEnumerable<TValue>
        where TKey : class
    {
        public static readonly ImmutableDictionary<TKey, TValue> Empty = new ImmutableDictionary<TKey, TValue>();
        public readonly int Count;
        public readonly ImmutableAvlNode<TKey, TValue>[] Buckets;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableDictionary<TKey, TValue> Add(TKey key, TValue value)
        {
            return new ImmutableDictionary<TKey, TValue>(this, new KeyValue<TKey, TValue>(key, value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal TValue Get(TKey key)
        {
            int hashCode = RuntimeHelpers.GetHashCode(key);
            int bucketIndex = hashCode & (Buckets.Length - 1);
            ImmutableAvlNode<TKey, TValue> avlNode = Buckets[bucketIndex];

            if (ReferenceEquals(avlNode.KeyValue.Key, key))
            {
                return avlNode.KeyValue.Value;
            }

            while (avlNode.Height != 0 && avlNode.KeyValue.HashCode != hashCode)
            {
                avlNode = hashCode < avlNode.KeyValue.HashCode ? avlNode.Left! : avlNode.Right!;
            }

            if (avlNode.Height != 0 && ReferenceEquals(avlNode.KeyValue.Key, key))
            {
                return avlNode.KeyValue.Value;
            }

            for (var i = 0; i < avlNode.Duplicates.Items.Length; i++)
            {
                if (ReferenceEquals(avlNode.Duplicates.Items[i].Key, key))
                {
                    return avlNode.Duplicates.Items[i].Value;
                }
            }

            return default!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ImmutableDictionary(ImmutableDictionary<TKey, TValue> previous, in KeyValue<TKey, TValue> keyValue)
        {
            this.Count = previous.Count + 1;
            if (previous.Count >= previous.Buckets.Length)
            {
                this.Buckets = new ImmutableAvlNode<TKey, TValue>[previous.Buckets.Length * 2];
                this.AddExistingValues(previous);
            }
            else
            {
                this.Buckets = new ImmutableAvlNode<TKey, TValue>[previous.Buckets.Length];
                for (var i = 0; i < previous.Buckets.Length; i++)
                {
                    this.Buckets[i] = previous.Buckets[i];
                }
            }

            int bucketIndex = keyValue.HashCode & (this.Buckets.Length - 1);
            this.Buckets[bucketIndex] = this.Buckets[bucketIndex].Add(in keyValue);
        }

        private ImmutableDictionary()
        {
            this.Buckets = new ImmutableAvlNode<TKey, TValue>[2];
            this.Buckets[0] = ImmutableAvlNode<TKey, TValue>.Empty;
            this.Buckets[1] = ImmutableAvlNode<TKey, TValue>.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddExistingValues(ImmutableDictionary<TKey, TValue> previous)
        {
            for (var i = 0; i < this.Buckets.Length; i++)
            {
                this.Buckets[i] = ImmutableAvlNode<TKey, TValue>.Empty;
            }

            foreach (ImmutableAvlNode<TKey, TValue> avlNode in previous.Buckets)
            {
                if (avlNode.IsEmpty) continue;
                ImmutableAvlNode<TKey, TValue> current = avlNode;
                var stack = new Stack<ImmutableAvlNode<TKey, TValue>>();

                while (true)
                {
                    FillBucket(current.KeyValue);

                    for (var i = 0; i < current.Duplicates.Items.Length; i++)
                    {
                        FillBucket(current.Duplicates.Items[i]);
                    }

                    if (!current.Left!.IsEmpty)
                        stack.Push(current.Left);
                    if (!current.Right!.IsEmpty)
                        stack.Push(current.Right);
                    if (stack.Count == 0) break;
                    current = stack.Pop();
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FillBucket(KeyValue<TKey, TValue> keyValue)
        {
            int bucketIndex = keyValue.HashCode & (this.Buckets.Length - 1);
            this.Buckets[bucketIndex] = this.Buckets[bucketIndex].Add(in keyValue);
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (ImmutableAvlNode<TKey, TValue> avlNode in Buckets)
            {
                if (avlNode.IsEmpty) continue;
                ImmutableAvlNode<TKey, TValue> current = avlNode;
                var stack = new Stack<ImmutableAvlNode<TKey, TValue>>();

                while (true)
                {
                    yield return current.KeyValue.Value;

                    for (var i = 0; i < current.Duplicates.Items.Length; i++)
                    {
                        yield return current.Duplicates.Items[i].Value;
                    }

                    if (!current.Left!.IsEmpty)
                        stack.Push(current.Left);
                    if (!current.Right!.IsEmpty)
                        stack.Push(current.Right);
                    if (stack.Count == 0) break;
                    current = stack.Pop();
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
