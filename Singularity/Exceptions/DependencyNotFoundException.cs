using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public class DependencyNotFoundException : SingularityException
    {
		public Type Type { get; }

        public DependencyNotFoundException()
        {
        }

        internal DependencyNotFoundException(Type type) : base(GetMessage(type))
		{
			Type = type;
		}

        public DependencyNotFoundException(Type type, Exception inner) : base(GetMessage(type), inner)
        {
            Type = type;
        }

        private static string GetMessage(Type type)
        {
            return $"Could not find dependency {type}";
        }

        protected DependencyNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
