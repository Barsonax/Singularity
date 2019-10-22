﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Singularity.Collections;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Microsoft.DependencyInjection.Test
{
    public class BindingConfigTests
    {
        [Fact]
        public void RegisterServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IRepositoryTransient1, RepositoryTransient1>();

            var container = new Container(builder =>
            {
                builder.RegisterServices(serviceCollection);
            });

            RegistrationStore registrationStore = container.Registrations;

            KeyValuePair<Type, Registration> registration = Assert.Single(registrationStore.Registrations);
            Assert.Empty(registrationStore.Decorators);
            ServiceBinding serviceBinding = Assert.Single(registration.Value.Bindings);
            Assert.Equal(typeof(IRepositoryTransient1), registration.Key);
            Assert.Equal(typeof(RepositoryTransient1), serviceBinding.Expression?.Type);
        }
    }
}
