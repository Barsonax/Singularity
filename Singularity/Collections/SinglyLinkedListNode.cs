using System;
using System.Collections;
using System.Collections.Generic;

namespace Singularity.Collections
{
    internal sealed class SinglyLinkedListNode<T> : IEnumerable<T>
    {
        public readonly T Value;
        public readonly SinglyLinkedListNode<T>? Next;

        public SinglyLinkedListNode(SinglyLinkedListNode<T>? next, in T value)
        {
            Next = next;
            Value = value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            SinglyLinkedListNode<T>? current = this;
            while (current != null)
            {
                yield return current.Value;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}