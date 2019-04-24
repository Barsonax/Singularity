namespace Singularity.Collections
{
    internal sealed class SinglyLinkedListNode<T>
    {
        public static readonly SinglyLinkedListNode<T> Empty = new SinglyLinkedListNode<T>();
        public readonly T Value;
        public readonly SinglyLinkedListNode<T> Next;

        private SinglyLinkedListNode()
        {
            Next = this;
        }
        public SinglyLinkedListNode(SinglyLinkedListNode<T> next, in T value)
        {
            Next = next;
            Value = value;
        }
    }
}