using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public class DependencyResolveException : SingularityException
    {
        public DependencyResolveException(Type type, ServiceBinding serviceBinding, Exception inner) : base($"Failed to resolve dependency {type} for registration {serviceBinding.BindingMetadata}", inner)
        {
        }

        protected DependencyResolveException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
