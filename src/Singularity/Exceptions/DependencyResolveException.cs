using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    /// <summary>
    /// Thrown if the service was found but it was not possible to resolve it.
    /// This could be due to missing dependencies of this service.
    /// </summary>
    [Serializable]
    public class DependencyResolveException : SingularityException
    {
        /// <summary>
        /// Constructs a new <see cref="DependencyResolveException"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="serviceBinding"></param>
        /// <param name="inner"></param>
        public DependencyResolveException(Type type, ServiceBinding serviceBinding, Exception inner) : base($"Failed to resolve dependency {type} for registration {serviceBinding.BindingMetadata}", inner)
        {
        }

        /// <inheritdoc />
        protected DependencyResolveException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
