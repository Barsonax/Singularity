using System;
using System.Threading;
using Singularity.Collections;

namespace Singularity
{
    internal class DisposeList
    {
        private Action<object> Action { get; }
        private SinglyLinkedListNode<object> _root;

        public DisposeList(Action<object> action)
        {
            Action = action;
            _root = SinglyLinkedListNode<object>.Empty;
        }

        public void Invoke()
        {
            SinglyLinkedListNode<object> root = _root;
            while (!ReferenceEquals(root, SinglyLinkedListNode<object>.Empty))
            {
                Action(root.Value);
                root = root.Next;
            }
        }

        public void Add(object obj)
        {
            SinglyLinkedListNode<object> initialValue, computedValue;
            do
            {
                initialValue = _root;
                computedValue = new SinglyLinkedListNode<object>(_root, obj);
            }
            while (initialValue != Interlocked.CompareExchange(ref _root, computedValue, initialValue));
        }
    }
}