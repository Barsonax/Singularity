using System.Collections;
using System.Collections.Generic;

namespace Singularity.Collections
{
    internal class ReadOnlyBindingCollection : IEnumerable<Binding>
    {
        private readonly SinglyLinkedListNode<Binding>? _bindings;
        public ReadOnlyBindingCollection(SinglyLinkedListNode<Binding>? bindings)
        {
            _bindings = bindings;
        }

        public ReadOnlyBindingCollection(IEnumerable<Binding> bindings)
        {
            SinglyLinkedListNode<Binding>? previous = null;
            foreach (Binding binding in bindings)
            {
                previous = new SinglyLinkedListNode<Binding>(previous, binding);
            }
            _bindings = previous;
        }

        public IEnumerator<Binding> GetEnumerator()
        {
            if(_bindings == null) yield break;
            SinglyLinkedListNode<Binding>? current = _bindings;
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
