using Microsoft.Extensions.DependencyInjection;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Microsoft.DependencyInjection.Test
{
    public class SingularityServiceProviderFactoryTests
    {
        [Fact]
        public void CreateContainerBuilder()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<ISingleton1, Singleton1>();

            //ARRANGE
            var factory = new SingularityServiceProviderFactory();
            var builder = factory.CreateBuilder(serviceCollection);
            var container = factory.CreateServiceProvider(builder);

            //ACT
            var singleton1 = container.GetService<ISingleton1>();

            //ASSERT
            Assert.IsType<Singleton1>(singleton1);
        }
    }
}
