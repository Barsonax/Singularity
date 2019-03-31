using System;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class Disposing
    {
        [Fact]
        public void GetInstance_PerContainerLifetimeAndOverride_IsDisposed()
        {
            var config = new BindingConfig();
            config.Register<IDisposable, Disposable>().With(CreationMode.Singleton).OnDeath(x => x.Dispose());

            var container = new Container(config);

            var topLevelInstance = container.GetInstance<IDisposable>();
            Assert.NotNull(topLevelInstance);
            Assert.Equal(typeof(Disposable), topLevelInstance.GetType());

            {
                var nestedConfig = new BindingConfig();
                nestedConfig.Register<IDisposable, Disposable>().With(CreationMode.Singleton).OnDeath(x => x.Dispose());
                Container nestedContainer = container.GetNestedContainer(nestedConfig, new Scoped());
                var nestedInstance = nestedContainer.GetInstance<IDisposable>();

                Assert.NotNull(nestedInstance);
                Assert.Equal(typeof(Disposable), nestedInstance.GetType());

                var castednestedInstance = (Disposable)nestedInstance;
                Assert.False(castednestedInstance.IsDisposed);
                nestedContainer.Dispose();
                Assert.True(castednestedInstance.IsDisposed);
            }

            var castedTopLevelInstance = (Disposable)topLevelInstance;
            Assert.False(castedTopLevelInstance.IsDisposed);
            container.Dispose();
            Assert.True(castedTopLevelInstance.IsDisposed);
        }

        [Fact]
        public void GetInstance_PerContainerLifetime_IsDisposedInTopLevel()
        {
            var config = new BindingConfig();
            config.Register<IDisposable, Disposable>().With(CreationMode.Singleton).OnDeath(x => x.Dispose());

            var container = new Container(config);

            var topLevelInstance = container.GetInstance<IDisposable>();
            Assert.NotNull(topLevelInstance);
            Assert.Equal(typeof(Disposable), topLevelInstance.GetType());

            {
                var nestedConfig = new BindingConfig();

                Container nestedContainer = container.GetNestedContainer(nestedConfig);
                var nestedInstance = nestedContainer.GetInstance<IDisposable>();

                Assert.NotNull(nestedInstance);
                Assert.Equal(typeof(Disposable), nestedInstance.GetType());

                var castednestedInstance = (Disposable)nestedInstance;
                Assert.False(castednestedInstance.IsDisposed);
                nestedContainer.Dispose();
                Assert.False(castednestedInstance.IsDisposed);
            }

            var castedTopLevelInstance = (Disposable)topLevelInstance;
            Assert.False(castedTopLevelInstance.IsDisposed);
            container.Dispose();
            Assert.True(castedTopLevelInstance.IsDisposed);
        }

        [Fact]
        public void GetInstance_PerCallLifetime_IsDisposedInTopLevel()
        {
            var config = new BindingConfig();
            config.Register<IDisposable, Disposable>().OnDeath(x => x.Dispose());

            var container = new Container(config);

            var topLevelInstance = container.GetInstance<IDisposable>();
            Assert.NotNull(topLevelInstance);
            Assert.Equal(typeof(Disposable), topLevelInstance.GetType());

            {
                var nestedConfig = new BindingConfig();

                Container nestedContainer = container.GetNestedContainer(nestedConfig, new Scoped());
                var nestedInstance = nestedContainer.GetInstance<IDisposable>();

                Assert.NotNull(nestedInstance);
                Assert.Equal(typeof(Disposable), nestedInstance.GetType());
                Assert.NotEqual(nestedInstance, topLevelInstance);

                var castednestedInstance = (Disposable)nestedInstance;
                Assert.False(castednestedInstance.IsDisposed);
                nestedContainer.Dispose();
                Assert.True(castednestedInstance.IsDisposed);
            }

            var castedTopLevelInstance = (Disposable)topLevelInstance;
            Assert.False(castedTopLevelInstance.IsDisposed);
            container.Dispose();
            Assert.True(castedTopLevelInstance.IsDisposed);
        }

        [Fact]
        public void GetInstance_PerContainerLifetimeAndNestedContainerDecorator_IsDisposed()
        {
            var config = new BindingConfig();
            config.Register<IDisposable, Disposable>().With(CreationMode.Singleton).OnDeath(x => x.Dispose());

            var container = new Container(config);
            var topLevelInstance = container.GetInstance<IDisposable>();

            Assert.NotNull(topLevelInstance);
            Assert.Equal(typeof(Disposable), topLevelInstance.GetType());
            {
                var nestedConfig = new BindingConfig();
                nestedConfig.Decorate<IDisposable, DisposableDecorator>();
                Container nestedContainer = container.GetNestedContainer(nestedConfig);
                var nestedInstance = nestedContainer.GetInstance<IDisposable>();

                Assert.NotNull(nestedInstance);
                Assert.Equal(typeof(DisposableDecorator), nestedInstance.GetType());
                var disposableDecorator = (DisposableDecorator)nestedInstance;
                Assert.Equal(typeof(Disposable), disposableDecorator.Disposable.GetType());
                var value = (Disposable)disposableDecorator.Disposable;

                Assert.Equal(topLevelInstance, value);
                Assert.False(value.IsDisposed);
                container.Dispose();
                Assert.True(value.IsDisposed);
            }
        }
    }
}
