using System;
using Microsoft.Extensions.DependencyInjection;
using Singularity.Bindings;
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

            var config = new BindingConfig();
            config.RegisterServices(serviceCollection);

            ReadOnlyBindingConfig readOnlyBindingConfig = config.GetDependencies();

            ReadonlyRegistration registration = Assert.Single(readOnlyBindingConfig.Registrations);
            Assert.Empty(readOnlyBindingConfig.Decorators);
            Type type = Assert.Single(registration.DependencyTypes);
            Binding binding = Assert.Single(registration.Bindings);
            Assert.Equal(typeof(IRepositoryTransient1), type);
            Assert.Equal(typeof(RepositoryTransient1), binding.Expression?.Type);
        }
    }
}
