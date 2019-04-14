using System;
using Microsoft.Extensions.DependencyInjection;

namespace Singularity.Microsoft.DependencyInjection
{
    public class SingularityServiceScope : IServiceScope, IServiceProvider
    {
        private readonly Scoped _scope;

        public SingularityServiceScope(SingularityServiceProvider container)
        {
            _scope = container.BeginScope();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        public IServiceProvider ServiceProvider => this;
        public object GetService(Type serviceType)
        {
            return _scope.GetInstance(serviceType);
        }
    }
}