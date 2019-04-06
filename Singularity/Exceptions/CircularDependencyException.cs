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

		internal CircularDependencyException(IReadOnlyList<Type> cycle) : base($"{cycle.First()} has circular dependencies! ({string.Join("->", cycle.Select(x => x.Name))})")
		{
			Cycle = cycle;
		}

        public CircularDependencyException()
        {
        }

        protected CircularDependencyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}