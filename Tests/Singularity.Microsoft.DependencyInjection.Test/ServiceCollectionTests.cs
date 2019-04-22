using System;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Singularity.Collections;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Microsoft.DependencyInjection.Test
{
    public class ServiceCollectionTests
    {
        [Fact]
        public void RegisterThroughServiceCollection()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IRepositoryTransient1, RepositoryTransient1>();
            serviceCollection.AddTransient<IRepositoryTransient2, RepositoryTransient2>();
            serviceCollection.AddTransient<IRepositoryTransient3, RepositoryTransient3>();
            serviceCollection.AddTransient<ISingleton1, Singleton1>();
            serviceCollection.AddScoped<IScopedService, ScopedService>();

            var config = new BindingConfig();
            config.RegisterServices(serviceCollection);

            IServiceProvider container = config.CreateServiceProvider();

            var factory = (IServiceScopeFactory)container.GetService(typeof(IServiceScopeFactory));

            using (IServiceScope scope = factory.CreateScope())
            {
                object controller = scope.ServiceProvider.GetService(typeof(TestController1));
            }
            Assert.Equal(1, TestController1.Instances);
            Assert.Equal(1, TestController1.DisposeCount);
        }

        [Fact]
        public void ServiceProviderIsRegistered()
        {
            //ARRANGE
            var config = new BindingConfig();

            //ACT
            IServiceProvider container = config.CreateServiceProvider();

            //ASSERT
            Assert.IsType<SingularityServiceProvider>(container);
        }

        [Fact]
        public void Register_Resolve_Transient()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IDisposable, TrackableDisposable>();
            var config = new BindingConfig();
            var tracker = new Tracker();
            config.Register<ITracker>().Inject(() => tracker);
            config.RegisterServices(serviceCollection);

            var container = new Container(config, SingularitySettings.Microsoft);

            //ACT
            var instance = container.GetInstance<IDisposable>();
            int disposeCountBefore = tracker.DisposeCount;
            container.Dispose();
            int disposeCountAfter = tracker.DisposeCount;

            //ASSERT
            Assert.IsType<TrackableDisposable>(instance);
            Assert.Equal(0, disposeCountBefore);
            Assert.Equal(1, disposeCountAfter);
        }

        [Fact]
        public void Register_Resolve_Scoped()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<IDisposable, TrackableDisposable>();
            var config = new BindingConfig();
            var tracker = new Tracker();
            config.Register<ITracker>().Inject(() => tracker);
            config.RegisterServices(serviceCollection);

            var container = new Container(config, SingularitySettings.Microsoft);

            //ACT
            int disposeCountBefore;
            IDisposable instance1, instance2;
            using (Scoped scope = container.BeginScope())
            {
                instance1 = scope.GetInstance<IDisposable>();
                instance2 = scope.GetInstance<IDisposable>();
                disposeCountBefore = tracker.DisposeCount;
            }
            int disposeCountAfter = tracker.DisposeCount;

            //ASSERT
            Assert.Same(instance1, instance2);
            Assert.IsType<TrackableDisposable>(instance1);
            Assert.Equal(0, disposeCountBefore);
            Assert.Equal(1, disposeCountAfter);
        }

        [Fact]
        public void Register_Resolve_Singleton()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IDisposable, TrackableDisposable>();
            var config = new BindingConfig();
            var tracker = new Tracker();
            config.Register<ITracker>().Inject(() => tracker);
            config.RegisterServices(serviceCollection);

            var container = new Container(config, SingularitySettings.Microsoft);

            //ACT

            var instance1 = container.GetInstance<IDisposable>();
            var instance2 = container.GetInstance<IDisposable>();
            int disposeCountBefore = tracker.DisposeCount;
            container.Dispose();
            int disposeCountAfter = tracker.DisposeCount;

            //ASSERT
            Assert.Same(instance1, instance2);
            Assert.IsType<TrackableDisposable>(instance1);
            Assert.Equal(0, disposeCountBefore);
            Assert.Equal(1, disposeCountAfter);
        }

        [Fact]
        public void Register_Resolve_Transient_Factory()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            var tracker = new Tracker();
            serviceCollection.AddTransient(typeof(IDisposable), s => new TrackableDisposable(tracker));
            var config = new BindingConfig();

            config.Register<ITracker>().Inject(() => tracker);
            config.Register<IServiceProvider, SingularityServiceProvider>();
            Container container = null;
            config.Register<Container>().Inject(() => container).With(DisposeBehavior.Never);
            config.RegisterServices(serviceCollection);

            container = new Container(config, SingularitySettings.Microsoft);

            //ACT
            var instance = container.GetInstance<IDisposable>();
            var foo = container.GetInstance<Expression<Func<IDisposable>>>();
            int disposeCountBefore = tracker.DisposeCount;
            container.Dispose();
            int disposeCountAfter = tracker.DisposeCount;

            //ASSERT
            Assert.IsType<TrackableDisposable>(instance);
            Assert.Equal(0, disposeCountBefore);
            Assert.Equal(1, disposeCountAfter);
        }
    }
}
