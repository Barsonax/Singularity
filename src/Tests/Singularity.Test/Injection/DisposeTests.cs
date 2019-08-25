using System;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class DisposeTests
    {
        [Fact]
        public void GetInstance_PerContainerLifetime_IsDisposed()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IDisposable, Disposable>(c => c
                    .With(Lifetimes.PerContainer)
                    .With(ServiceAutoDispose.Always));
            });

            //ACT
            var disposable = container.GetInstance<IDisposable>();

            //ASSERT
            var value = Assert.IsType<Disposable>(disposable);

            Assert.False(value.IsDisposed);
            container.Dispose();
            Assert.True(value.IsDisposed);
        }

        [Fact]
        public void GetInstance_AutoDispose_IsDisposed()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IDisposable, Disposable>(c => c
                    .With(Lifetimes.PerContainer));
            }, new SingularitySettings { AutoDispose = new[] { Lifetimes.PerContainer } });

            //ACT
            var disposable = container.GetInstance<IDisposable>();

            //ASSERT
            var value = Assert.IsType<Disposable>(disposable);

            Assert.False(value.IsDisposed);
            container.Dispose();
            Assert.True(value.IsDisposed);
        }

        [Fact]
        public void GetInstance_AutoDispose_NoDisposable_IsNotDisposed()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ITestService10, TestService10>(c => c
                    .With(Lifetimes.PerContainer));
            }, new SingularitySettings { AutoDispose = new[] { Lifetimes.PerContainer } });

            //ACT
            var testService10 = container.GetInstance<ITestService10>();
            container.Dispose();

            //ASSERT
            var value = Assert.IsType<TestService10>(testService10);
        }

        [Fact]
        public void GetInstance_PerCallLifetime_IsDisposed()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IDisposable, Disposable>(c => c
                    .With(ServiceAutoDispose.Always));
            });

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
            var container = new Container(builder =>
            {
                builder.Register<IDisposable, Disposable>(c => c
                    .With(ServiceAutoDispose.Always));
                builder.Decorate<IDisposable, DisposableDecorator>();
            });

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
            var container = new Container(builder =>
            {
                builder.Register<IDisposable, Disposable>(c => c
                    .With(Lifetimes.PerContainer)
                    .With(ServiceAutoDispose.Always));
            });
            Container nestedContainer = container.GetNestedContainer();

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
            var container = new Container(builder =>
            {
                builder.Register<IDisposable, Disposable>(c => c
                    .With(ServiceAutoDispose.Always));
            });
            Container nestedContainer = container.GetNestedContainer();

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
