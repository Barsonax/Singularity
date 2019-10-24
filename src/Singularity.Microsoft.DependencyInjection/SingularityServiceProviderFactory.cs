﻿using System;
using Microsoft.Extensions.DependencyInjection;

namespace Singularity.Microsoft.DependencyInjection
{
    /// <inheritdoc />
    public class SingularityServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        private readonly SingularitySettings? _settings;

        /// <summary>
        /// Creates a new factory with the optionally provided settings
        /// </summary>
        /// <param name="settings"></param>
        public SingularityServiceProviderFactory(SingularitySettings? settings = null)
        {
            _settings = settings;
        }

        /// <inheritdoc />
        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            return services.CreateContainerBuilder(_settings);
        }

        /// <inheritdoc />
        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            var container = new Container(containerBuilder);
            return container.GetInstance<IServiceProvider>();
        }
    }
}