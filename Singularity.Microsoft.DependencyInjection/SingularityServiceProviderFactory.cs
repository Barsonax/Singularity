using System;
using System.ComponentModel.Design;
using Microsoft.Extensions.DependencyInjection;

namespace Singularity.Microsoft.DependencyInjection
{
    public class SingularityServiceProviderFactory : IServiceProviderFactory<IServiceContainer>
    {
        private IServiceCollection services;

        public IServiceContainer CreateBuilder(IServiceCollection serviceCollection)
        {
            this.services = serviceCollection;
            return new ServiceContainer();
        }

        public IServiceProvider CreateServiceProvider(IServiceContainer serviceContainer)
        {
            var config = new BindingConfig();
            config.RegisterServices(services);
            return config.CreateServiceProvider();
        }
    }
}