using Microsoft.Extensions.DependencyInjection;

namespace Singularity
{
    /// <summary>
    /// <see cref="IServiceScopeFactory"/> implementation for the singularity container.
    /// Wraps a <see cref="Container"/> so that it can be used as a <see cref="IServiceScopeFactory"/>
    /// </summary>
    public class SingularityServiceScopeFactory : IServiceScopeFactory
    {
        private readonly Container _container;

        /// <summary>
        /// Creates a new service scope factory with the provided <see cref="Container"/>.
        /// </summary>
        /// <param name="container"></param>
        public SingularityServiceScopeFactory(Container container)
        {
            _container = container;
        }

        /// <inheritdoc />
        public IServiceScope CreateScope()
        {
            return new SingularityServiceScope(_container.BeginScope());
        }
    }
}