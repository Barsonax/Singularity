using System.Runtime.CompilerServices;

namespace Singularity.Collections
{
    internal static class HashHelpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode(object item)
        {
            //return item.GetHashCode();
            return RuntimeHelpers.GetHashCode(item);
        }
    }
}