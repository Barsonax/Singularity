using Microsoft.Extensions.DependencyInjection;

namespace Singularity.Microsoft.DependencyInjection
{
    public class SingularityServiceScopeFactory : IServiceScopeFactory
    {
        private readonly Container _container;

        public SingularityServiceScopeFactory(Container container)
        {
            _container = container;
        }

        public IServiceScope CreateScope()
        {
            return new SingularityServiceScope(_container.BeginScope());
        }
    }
}