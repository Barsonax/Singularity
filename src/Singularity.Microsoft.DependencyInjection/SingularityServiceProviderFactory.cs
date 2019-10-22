using System;
using Microsoft.Extensions.DependencyInjection;

namespace Singularity.Microsoft.DependencyInjection
{
    public class SingularityServiceProviderFactory : IServiceProviderFactory<Container>
    {
        /// <inheritdoc />
        public Container CreateBuilder(IServiceCollection services)
        {
            return services.BuildSingularityContainer();
        }

        /// <inheritdoc />
        public IServiceProvider CreateServiceProvider(Container containerBuilder)
        {
            return containerBuilder.GetInstance<IServiceProvider>();
        }
    }
}