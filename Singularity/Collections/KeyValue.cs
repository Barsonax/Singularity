namespace Singularity.Collections
{
    public struct KeyValue<TKey, TValue>
    {
        public readonly TKey Key;
        public readonly TValue Value;

        public KeyValue(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}
