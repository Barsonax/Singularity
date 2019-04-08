using System;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class Disposing
    {
        [Fact]
        public void GetInstance_PerContainerLifetime_IsDisposed()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IDisposable, Disposable>().With(CreationMode.Singleton).OnDeath(x => x.Dispose());

            var container = new Container(config);

            //ACT
            var disposable = container.GetInstance<IDisposable>();

            //ASSERT
            var value = Assert.IsType<Disposable>(disposable);

            Assert.False(value.IsDisposed);
            container.Dispose();
            Assert.True(value.IsDisposed);
        }

        [Fact]
        public void GetInstance_PerCallLifetime_IsDisposed()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IDisposable, Disposable>().OnDeath(x => x.Dispose());

            var container = new Container(config);

            //ACT
            var disposable = container.GetInstance<IDisposable>();

            //ASSERT
            var value = Assert.IsType<Disposable>(disposable);

            Assert.False(value.IsDisposed);
            container.Dispose();
            Assert.True(value.IsDisposed);
        }

        [Fact]
        public void GetInstance_Decorator_IsDisposed()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IDisposable, Disposable>().OnDeath(x => x.Dispose());
            config.Decorate<IDisposable, DisposableDecorator>();

            var container = new Container(config);

            //ACT
            var disposable = container.GetInstance<IDisposable>();

            //ASSERT
            var disposableDecorator = Assert.IsType<DisposableDecorator>(disposable);
            var value = Assert.IsType<Disposable>(disposableDecorator.Disposable);

            Assert.False(value.IsDisposed);
            container.Dispose();
            Assert.True(value.IsDisposed);
        }

        [Fact]
        public void GetInstance_PerContainerLifetime_IsDisposedInTopLevel()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IDisposable, Disposable>().With(CreationMode.Singleton).OnDeath(x => x.Dispose());
            var nestedConfig = new BindingConfig();

            var container = new Container(config);
            Container nestedContainer = container.GetNestedContainer(nestedConfig);

            //ACT
            var topLevelInstance = container.GetInstance<IDisposable>();
            var nestedInstance = nestedContainer.GetInstance<IDisposable>();

            //ASSERT
            var castedTopLevelInstance = Assert.IsType<Disposable>(topLevelInstance);
            var castednestedInstance = Assert.IsType<Disposable>(nestedInstance);

            Assert.False(castednestedInstance.IsDisposed);
            nestedContainer.Dispose();
            Assert.False(castednestedInstance.IsDisposed);

            Assert.False(castedTopLevelInstance.IsDisposed);
            container.Dispose();
            Assert.True(castedTopLevelInstance.IsDisposed);
        }

        [Fact]
        public void GetInstance_PerCallLifetime_IsDisposedInTopLevel()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IDisposable, Disposable>().OnDeath(x => x.Dispose());
            var nestedConfig = new BindingConfig();

            var container = new Container(config);
            Container nestedContainer = container.GetNestedContainer(nestedConfig, new Scoped("nestedScope"));

            //ACT
            var topLevelInstance = container.GetInstance<IDisposable>();
            var nestedInstance = nestedContainer.GetInstance<IDisposable>();

            //ASSERT
            var castedTopLevelInstance = Assert.IsType<Disposable>(topLevelInstance);
            var castednestedInstance = Assert.IsType<Disposable>(nestedInstance);
            Assert.NotSame(nestedInstance, topLevelInstance);

            Assert.False(castednestedInstance.IsDisposed);
            nestedContainer.Dispose();
            Assert.True(castednestedInstance.IsDisposed);

            Assert.False(castedTopLevelInstance.IsDisposed);
            container.Dispose();
            Assert.True(castedTopLevelInstance.IsDisposed);
        }
    }
}
