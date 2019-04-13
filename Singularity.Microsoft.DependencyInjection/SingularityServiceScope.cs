using System;
using Microsoft.Extensions.DependencyInjection;

namespace Singularity.Microsoft.DependencyInjection
{
    public class SingularityServiceScope : IServiceScope
    {
        private readonly Scoped _scope;

        public SingularityServiceScope(SingularityServiceProvider container)
        {
            _scope = container.BeginScope();
            ServiceProvider = container;
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        public IServiceProvider ServiceProvider { get; }
    }
}