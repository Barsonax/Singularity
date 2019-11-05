using System;

namespace Singularity.Collections
{
    internal static class SinglyLinkedListNodeTypeCache<T>
    {
        public static readonly SinglyLinkedListNode<Type> Instance = new SinglyLinkedListNode<Type>(typeof(T));
    }
}