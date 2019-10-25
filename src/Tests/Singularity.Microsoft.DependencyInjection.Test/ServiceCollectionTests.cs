using System;
using System.Linq.Expressions;
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
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IRepositoryTransient1, RepositoryTransient1>();
            serviceCollection.AddTransient<IRepositoryTransient2, RepositoryTransient2>();
            serviceCollection.AddTransient<IRepositoryTransient3, RepositoryTransient3>();
            serviceCollection.AddTransient<IRepositoryTransient4, RepositoryTransient4>();
            serviceCollection.AddTransient<IRepositoryTransient5, RepositoryTransient5>();
            serviceCollection.AddSingleton(typeof(ISingleton1), new Singleton1());
            serviceCollection.AddScoped<IScopedService1, ScopedService1>();
            serviceCollection.AddScoped<IScopedService2, ScopedService2>();
            serviceCollection.AddScoped<IScopedService3, ScopedService3>();
            serviceCollection.AddScoped<IScopedService4, ScopedService4>();
            serviceCollection.AddScoped<IScopedService5, ScopedService5>();

            var container = new Container(builder =>
            {
                builder.RegisterServices(serviceCollection);
                builder.RegisterServiceProvider();
            }, SingularitySettings.Microsoft).GetInstance<IServiceProvider>();

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
            var container = new Container(builder =>
            {
                builder.RegisterServiceProvider();
            });

            //ACT
            IServiceProvider serviceProvider = container.GetInstance<IServiceProvider>();

            //ASSERT
            Assert.IsType<SingularityServiceProvider>(serviceProvider);
        }

        [Fact]
        public void CreateContainerBuilder()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<ISingleton1, Singleton1>();

            //ARRANGE
            var builder = serviceCollection.CreateContainerBuilder();
            var container = new Container(builder);

            //ACT
            IServiceProvider serviceProvider = container.GetInstance<IServiceProvider>();

            //ASSERT
            Assert.IsType<SingularityServiceProvider>(serviceProvider);
        }

        [Fact]
        public void Register_Resolve_Transient()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IDisposable, TrackableDisposable>();

            var tracker = new Tracker();

            var container = new Container(builder =>
            {
                builder.Register<ITracker>(c => c.Inject(() => tracker));
                builder.RegisterServices(serviceCollection);
            }, SingularitySettings.Microsoft);

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
            var tracker = new Tracker();

            var container = new Container(builder =>
            {
                builder.Register<ITracker>(c => c.Inject(() => tracker));
                builder.RegisterServices(serviceCollection);
            }, SingularitySettings.Microsoft);

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

            var tracker = new Tracker();

            var container = new Container(builder =>
            {
                builder.Register<ITracker>(c => c.Inject(() => tracker));
                builder.RegisterServices(serviceCollection);
            }, SingularitySettings.Microsoft);

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

            var container = new Container(builder =>
            {
                builder.RegisterServices(serviceCollection);
                builder.RegisterServiceProvider();

            }, SingularitySettings.Microsoft);

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
