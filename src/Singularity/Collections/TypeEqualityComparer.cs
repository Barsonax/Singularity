using System.Collections.Generic;

namespace Singularity.Test.Settings
{
    public class TypeEqualityComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            if (x == null && y == null) return true;
            else if (x == null) return false;
            else if (y == null) return false;

            return x.GetType() == y.GetType();
        }

        public int GetHashCode(T obj)
        {
            return obj.GetType().GetHashCode();
        }
    }
}
