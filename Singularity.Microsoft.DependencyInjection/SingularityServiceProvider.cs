using System;

namespace Singularity.Microsoft.DependencyInjection
{
    public class SingularityServiceProvider : IServiceProvider
    {
        private readonly Container _container;

        public SingularityServiceProvider(Container container)
        {
            _container = container;
        }

        public object GetService(Type type)
        {
            return _container.GetInstance(type);
        }
    }
}