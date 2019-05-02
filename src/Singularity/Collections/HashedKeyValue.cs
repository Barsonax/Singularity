namespace Singularity.Collections
{
    internal readonly struct HashedKeyValue<TKey, TValue>
        where TKey : class
    {
        public readonly TKey Key;
        public readonly TValue Value;
        public readonly int HashCode;

        public HashedKeyValue(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            HashCode = HashHelpers.GetHashCode(key);
        }
    }
}
