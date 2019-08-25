using System.Collections.Generic;

namespace Singularity
{
    public class LifetimeCollection
    {
        public static LifetimeCollection Empty = new LifetimeCollection(new ILifetime[0]);
        private HashSet<ILifetime> Lifetimes { get; }

        private LifetimeCollection(ILifetime[] lifetimes)
        {
            Lifetimes = new HashSet<ILifetime>(lifetimes);
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
    }
}