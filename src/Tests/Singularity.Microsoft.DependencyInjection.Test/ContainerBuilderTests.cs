using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Singularity.Collections;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Microsoft.DependencyInjection.Test
{
    public class ContainerBuilderTests
    {
        [Fact]
        public void RegisterServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IRepositoryTransient1, RepositoryTransient1>();

            var builder = new ContainerBuilder(builder =>
            {
                builder.RegisterServices(serviceCollection);
            });

            RegistrationStore registrationStore = builder.Registrations;

            Assert.Equal(2, registrationStore.Registrations.Count);

            var registration = registrationStore.Registrations[typeof(IRepositoryTransient1)];
            Assert.Empty(registrationStore.Decorators);
            ServiceBinding serviceBinding = Assert.Single(registration.Bindings);
            Assert.Equal(typeof(RepositoryTransient1), serviceBinding.Expression?.Type);
        }
    }
}
