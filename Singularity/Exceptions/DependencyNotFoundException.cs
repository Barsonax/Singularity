using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public class DependencyNotFoundException : SingularityException
    {
		public Type Type { get; }

		internal DependencyNotFoundException(Type type) : base($"Could not find dependency {type}")
		{
			Type = type;
		}

        public DependencyNotFoundException()
        {
        }

        protected DependencyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
