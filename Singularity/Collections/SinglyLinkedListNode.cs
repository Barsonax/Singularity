namespace Singularity.Collections
{
    public sealed class SinglyLinkedListNode<T>
    {
        public static readonly SinglyLinkedListNode<T> Empty = new SinglyLinkedListNode<T>();
        public readonly T Value;
        public readonly SinglyLinkedListNode<T> Next;

        private SinglyLinkedListNode() { }
        public SinglyLinkedListNode(SinglyLinkedListNode<T> next, T value)
        {
            Next = next;
            Value = value;
        }
    }
}