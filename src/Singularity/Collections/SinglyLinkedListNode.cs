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
    /// <typeparam name="T"></typeparam>
    public sealed class SinglyLinkedListNode<T> : IEnumerable<T>
    {
        /// <summary>
        /// The value of this node.
        /// </summary>
        public readonly T Value;

        /// <summary>
        /// The next node in the list.
        /// </summary>
        public readonly SinglyLinkedListNode<T>? Next;

        /// <summary>
        /// Creates a new list with the provided value.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SinglyLinkedListNode(in T value)
        {
            Value = value;
        }

        /// <summary>
        /// Adds a new node to a existing list with the provided value.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SinglyLinkedListNode(SinglyLinkedListNode<T>? next, in T value)
        {
            Next = next;
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

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
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
        public sealed class Enumerator : IEnumerator<T>
        {
            private SinglyLinkedListNode<T>? _node;

            /// <summary>
            /// The value of the current node.
            /// </summary>
            public T Current { get; private set; }

            object IEnumerator.Current => Current!;

            internal Enumerator(SinglyLinkedListNode<T> list)
            {
                _node = list;
                Current = default!;
            }

            /// <inheritdoc />
            public bool MoveNext()
            {
                if(_node != null)
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
            public void Dispose() { /* Nothing to dispose */ }
        }
    }

    /// <summary>
    /// Extensions for <see cref="SinglyLinkedListNode{T}"/>
    /// </summary>
    public static class SinglyLinkedListNodeExtensions
    {
        /// <summary>
        /// returns a new <see cref="SinglyLinkedListNode{T}"/> that points to the passed <paramref name="previous"/> node.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="previous"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SinglyLinkedListNode<T> Add<T>(this SinglyLinkedListNode<T>? previous, in T value)
        {
            return new SinglyLinkedListNode<T>(previous, in value);
        }

        /// <summary>
        /// Converts the passed <paramref name="collection"/> to a <see cref="SinglyLinkedListNode{T}"/>
        /// Note: reverses the order of the elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static SinglyLinkedListNode<T>? ToSinglyLinkedList<T>(this IEnumerable<T> collection)
        {
            SinglyLinkedListNode<T>? previous = null;
            foreach (var element in collection)
            {
                previous = new SinglyLinkedListNode<T>(previous, element);
            }

            return previous;
        }
    }
}