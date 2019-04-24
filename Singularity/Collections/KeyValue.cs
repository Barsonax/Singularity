using System.Runtime.CompilerServices;

namespace Singularity.Collections
{
    internal readonly struct KeyValue<TKey, TValue>
        where TKey : class
    {
        public readonly TKey Key;
        public readonly TValue Value;
        public readonly int HashCode;

        public KeyValue(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            HashCode = HashHelpers.GetHashCode(key);
        }
    }
}
