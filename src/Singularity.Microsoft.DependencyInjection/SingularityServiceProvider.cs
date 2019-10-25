using System;

namespace Singularity.Microsoft.DependencyInjection
{
    /// <summary>
    /// <see cref="IServiceProvider"/> implementation for the singularity container.
    /// Wraps a <see cref="Container"/> so that it can be used as a <see cref="IServiceProvider"/>
    /// </summary>
    public class SingularityServiceProvider : IServiceProvider
    {
        private readonly Container _container;

        /// <summary>
        /// Creates a new service provider with the provided <see cref="Container"/>.
        /// </summary>
        /// <param name="container"></param>
        public SingularityServiceProvider(Container container)
        {
            _container = container;
        }

        /// <inheritdoc />
        public object? GetService(Type serviceType)
        {
            return _container.GetInstanceOrDefault(serviceType);
        }
    }
}