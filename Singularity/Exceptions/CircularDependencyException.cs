using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public class CircularDependencyException : SingularityException
    {
		public IReadOnlyList<Type> Cycle { get; }

        public CircularDependencyException()
        {
        }

        internal CircularDependencyException(IReadOnlyList<Type> cycle) : base(GetMessage(cycle))
		{
			Cycle = cycle;
		}

        public CircularDependencyException(IReadOnlyList<Type> cycle, Exception inner) : base(GetMessage(cycle), inner)
        {
            Cycle = cycle;
        }

        private static string GetMessage(IReadOnlyList<Type> cycle)
        {
            return $"{cycle.First()} has circular dependencies! ({string.Join("->", cycle.Select(x => x.Name))})";
        }

        protected CircularDependencyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}