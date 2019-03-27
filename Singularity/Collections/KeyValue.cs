namespace Singularity.Collections
{
    internal struct KeyValue<TKey, TValue>
    {
        public readonly TKey Key;
        public readonly TValue Value;
        public readonly int HashCode;

        public KeyValue(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            HashCode = key!.GetHashCode();
        }
    }
}
