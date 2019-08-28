using System.Collections;
using System.Collections.Generic;

namespace Singularity.Collections
{
    public sealed class LifetimeCollection : IEnumerable<ILifetime>
    {
        public static LifetimeCollection Empty { get; } = new LifetimeCollection(new ILifetime[0]);
        internal static LifetimeComparer Comparer = new LifetimeComparer();
        private HashSet<ILifetime> Lifetimes { get; }

        private LifetimeCollection(ILifetime[] lifetimes)
        {
            Lifetimes = new HashSet<ILifetime>(lifetimes, Comparer);
        }

        /// <summary>
        /// Implicit operator for a nicer API
        /// </summary>
        /// <param name="lifetimes"></param>
        /// <returns></returns>
        public static implicit operator LifetimeCollection(ILifetime[] lifetimes)
        {
            return new LifetimeCollection(lifetimes);
        }

        public bool Contains(ILifetime lifetime)
        {
            return Lifetimes.Contains(lifetime);
        }

        IEnumerator<ILifetime> IEnumerable<ILifetime>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private IEnumerator<ILifetime> GetEnumerator() => Lifetimes.GetEnumerator();

        internal sealed class LifetimeComparer : IEqualityComparer<ILifetime>
        {
            public bool Equals(ILifetime x, ILifetime y)
            {
                return y != null && x != null &&
                       (ReferenceEquals(x, y) || x.GetType() == y.GetType());
            }

            public int GetHashCode(ILifetime obj)
            {
                return obj.GetType().GetHashCode();
            }
        }
    }
}