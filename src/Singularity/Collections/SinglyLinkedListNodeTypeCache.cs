using System;

namespace Singularity.Collections
{
    public static class SinglyLinkedListNodeTypeCache<T>
    {
        public static readonly SinglyLinkedListNode<Type> Instance = new SinglyLinkedListNode<Type>(typeof(T));
    }
}