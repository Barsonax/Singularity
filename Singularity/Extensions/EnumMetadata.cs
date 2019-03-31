using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Singularity
{
    internal static class EnumMetadata<T>
        where T : struct, Enum
    {
        public static readonly ReadOnlyCollection<T> Values;

        static EnumMetadata()
        {
            var values = Enum.GetValues(typeof(T)).OfType<T>().ToArray();
            Values = new ReadOnlyCollection<T>(values);
        }

        public static bool IsValidValue(T value)
        {
            return Values.Contains(value);
        }
    }
}