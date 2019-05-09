using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Singularity.Collections
{
    /// <summary>
    /// A immutable singly linked list.
    /// Note that adding items to this list will reverse the order.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public sealed class SinglyLinkedListKeyNode<TKey, TValue> : IEnumerable<TValue>
    {
        public readonly TKey Key;

        /// <summary>
        /// The value of this node.
        /// </summary>
        public readonly TValue Value;

        /// <summary>
        /// The next node in the list.
        /// </summary>
        public readonly SinglyLinkedListKeyNode<TKey, TValue>? Next;

        /// <summary>
        /// Creates a new list with the provided value.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SinglyLinkedListKeyNode(in TKey key, in TValue value)
        {
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Adds a new node to a existing list with the provided value.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SinglyLinkedListKeyNode(SinglyLinkedListKeyNode<TKey, TValue>? next, in TKey key, in TValue value)
        {
            Next = next;
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Uses a more efficient enumerator when the type is known.
        /// </summary>
        /// <returns></returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Efficient enumerator.
        /// </summary>
        /// <returns></returns>
        public sealed class Enumerator : IEnumerator<TValue>
        {
            private SinglyLinkedListKeyNode<TKey, TValue>? _node;

            /// <summary>
            /// The value of the current node.
            /// </summary>
            public TValue Current { get; private set; }

            object IEnumerator.Current => Current!;

            internal Enumerator(SinglyLinkedListKeyNode<TKey, TValue> list)
            {
                _node = list;
                Current = default!;
            }

            /// <inheritdoc />
            public bool MoveNext()
            {
                if (_node != null)
                {
                    Current = _node.Value;
                    _node = _node.Next;
                    return true;
                }
                return false;
            }

            /// <inheritdoc />
            public void Reset()
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public void Dispose() { }
        }
    }

    internal static class SinglyLinkedListKeyNodeExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SinglyLinkedListKeyNode<TKey, TValue> Add<TKey, TValue>(this SinglyLinkedListKeyNode<TKey, TValue>? previous, in TKey key, in TValue value)
        {
            return new SinglyLinkedListKeyNode<TKey, TValue>(previous, in key, in value);
        }

        public static TValue GetOrDefault<TKey, TValue>(this SinglyLinkedListKeyNode<TKey, TValue>? list, TKey key)
        {
            while (list != null)
            {
                if (ReferenceEquals(list.Key, key)) return list.Value;
                list = list.Next!;
            }

            return default!;
        }
    }
}