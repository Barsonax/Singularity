using Microsoft.Extensions.DependencyInjection;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Microsoft.DependencyInjection.Test
{
    public class ServiceCollectionTests
    {
        [Fact]
        public void RegisterThroughServiceCollection()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<TestController1>();
            serviceCollection.AddTransient<TestController2>();
            serviceCollection.AddTransient<TestController3>();
            serviceCollection.AddTransient<IRepositoryTransient1, RepositoryTransient1>();
            serviceCollection.AddTransient<IRepositoryTransient2, RepositoryTransient2>();
            serviceCollection.AddTransient<IRepositoryTransient3, RepositoryTransient3>();
            serviceCollection.AddTransient<ISingleton1, Singleton1>();
            serviceCollection.AddScoped<IScopedService, ScopedService>();

            var config = new BindingConfig();
            config.RegisterServices(serviceCollection);

            var container = config.CreateServiceProvider();

            var factory = (IServiceScopeFactory)container.GetService(typeof(IServiceScopeFactory));

            using (var scope = factory.CreateScope())
            {
                var controller = scope.ServiceProvider.GetService(typeof(RepositoryTransient1));
            }
        }
    }
}
