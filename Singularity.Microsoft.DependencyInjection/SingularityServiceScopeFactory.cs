using System;
using Microsoft.Extensions.DependencyInjection;

namespace Singularity.Microsoft.DependencyInjection
{
    public class SingularityServiceScopeFactory : IServiceScopeFactory
    {
        private readonly SingularityServiceProvider _container;

        public SingularityServiceScopeFactory(SingularityServiceProvider container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public IServiceScope CreateScope()
        {
            return new SingularityServiceScope(_container);
        }
    }
}