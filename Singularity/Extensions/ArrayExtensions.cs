using System.Runtime.CompilerServices;

namespace Singularity
{
    internal static class ArrayExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Add<T>(this T[] array, T value)
        {
            var items = new T[array.Length + 1];
            for (var i = 0; i < array.Length; i++)
            {
                items[i] = array[i];
            }

            items[items.Length - 1] = value;
            return items;
        }
    }
}
