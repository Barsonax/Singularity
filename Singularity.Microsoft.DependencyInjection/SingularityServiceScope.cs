using System;
using Microsoft.Extensions.DependencyInjection;

namespace Singularity.Microsoft.DependencyInjection
{
    /// <summary>
    /// <see cref="IServiceScope"/> implementation for the singularity container.
    /// Wraps a <see cref="Scoped"/> so that it can be used as a <see cref="IServiceScope"/>
    /// </summary>
    public sealed class SingularityServiceScope : IServiceScope, IServiceProvider
    {
        private readonly Scoped _scope;

        /// <summary>
        /// Creates a new service provider with the provided <see cref="Scoped"/>.
        /// </summary>
        /// <param name="scope"></param>
        public SingularityServiceScope(Scoped scope)
        {
            _scope = scope;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _scope.Dispose();
        }

        /// <inheritdoc />
        public IServiceProvider ServiceProvider => this;

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            return _scope.GetInstance(serviceType);
        }
    }
}