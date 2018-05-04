using System;
using System.Collections.Generic;

namespace Singularity.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue TryGetDefaultValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            return default(TValue);
        }
    }
}
