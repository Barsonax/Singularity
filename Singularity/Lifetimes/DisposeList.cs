using System;
using System.Threading;
using Singularity.Collections;

namespace Singularity
{
    internal class DisposeList<T>
    {
        private Action<T> Action { get; }
        private SinglyLinkedListNode<T> _root;

        public DisposeList(Action<T> action)
        {
            Action = action;
            _root = SinglyLinkedListNode<T>.Empty;
        }

        public void Invoke()
        {
            SinglyLinkedListNode<T> root = _root;
            while (!ReferenceEquals(root, SinglyLinkedListNode<T>.Empty))
            {
                Action(root.Value);
                root = root.Next;
            }
        }

        public void Add(T obj)
        {
            SinglyLinkedListNode<T> initialValue, computedValue;
            do
            {
                initialValue = _root;
                computedValue = new SinglyLinkedListNode<T>(_root, obj);
            }
            while (initialValue != Interlocked.CompareExchange(ref _root, computedValue, initialValue));
        }
    }
}