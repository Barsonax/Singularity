using System;
using Singularity.Exceptions;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class ChildContainerTests
    {
        [Fact]
        public void GetNestedContainer_OverrideInChild_Throws()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ITransient1, Transient1>();
                builder.Register<ISingleton1, Singleton1>();
                builder.Register<ICombined1, Combined1>();
            });

            //ACT
            //ASSERT
            Assert.Throws<RegistrationAlreadyExistsException>(() =>
            {
                Container nestedContainer = container.GetNestedContainer(builder =>
                {
                    builder.Register<ITransient1, ScopedTransient>();
                });
            });
        }

        [Fact]
        public void GetInstance_Singleton_Disposable()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IDisposable, Disposable>(c => c.With(Lifetimes.PerContainer).With(ServiceAutoDispose.Always));
            });

            //ACT
            var childContainer = container.GetNestedContainer();

            var instance1 = container.GetInstance<Disposable>();
            var instance2 = childContainer.GetInstance<Disposable>();

            //ASSERT
            Assert.Equal(instance1, instance2);
            Assert.False(instance1.IsDisposed);
            childContainer.Dispose();
            Assert.False(instance1.IsDisposed);
            container.Dispose();
            Assert.True(instance1.IsDisposed);
        }

        [Fact]
        public void GetInstance_Scoped_Disposable()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IDisposable, Disposable>(c => c.With(Lifetimes.PerScope).With(ServiceAutoDispose.Always));
            });

            //ACT
            var childContainer = container.GetNestedContainer();

            var instance1 = container.GetInstance<Disposable>();
            var instance2 = childContainer.GetInstance<Disposable>();

            //ASSERT
            Assert.NotEqual(instance1, instance2);
            Assert.False(instance1.IsDisposed);
            Assert.False(instance2.IsDisposed);
            childContainer.Dispose();
            Assert.False(instance1.IsDisposed);
            Assert.True(instance2.IsDisposed);
            container.Dispose();
            Assert.True(instance1.IsDisposed);
        }

        [Fact]
        public void GetInstance_Singleton_MultiInterface()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ISingleton1, Singleton1>(c => c.With(Lifetimes.PerContainer));
            });

            //ACT
            var childContainer = container.GetNestedContainer();

            var instance0 = container.GetInstance<ISingleton1>();
            var instance1 = childContainer.GetInstance<ISingleton1>();
            var instance2 = childContainer.GetInstance<Singleton1>();

            //ASSERT
            Assert.Equal(instance0, instance1);
            Assert.Equal(instance0, instance2);
        }

        [Fact]
        public void GetInstance_Singleton_MultiInterface_Func()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ISingleton1, Singleton1>(c => c.With(Lifetimes.PerContainer));
            });

            //ACT
            var childContainer = container.GetNestedContainer();

            var instance0 = container.GetInstance<Func<ISingleton1>>().Invoke();
            var instance1 = childContainer.GetInstance<Func<ISingleton1>>().Invoke();
            var instance2 = childContainer.GetInstance<Func<Singleton1>>().Invoke();

            //ASSERT
            Assert.Equal(instance0, instance1);
            Assert.Equal(instance0, instance2);
        }

        [Fact]
        public void GetInstance_Scoped_MultiInterface()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ISingleton1, Singleton1>(c => c.With(Lifetimes.PerScope));
            });

            //ACT
            var childContainer = container.GetNestedContainer();

            var instance0 = container.GetInstance<ISingleton1>();
            var instance1 = childContainer.GetInstance<ISingleton1>();
            var instance2 = childContainer.GetInstance<Singleton1>();

            //ASSERT
            Assert.NotEqual(instance0, instance1);
            Assert.NotEqual(instance0, instance2);
            Assert.Equal(instance1, instance2);
        }

        [Fact]
        public void GetInstance_Scoped_MultiInterface_Func()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ISingleton1, Singleton1>(c => c.With(Lifetimes.PerScope));
            });

            //ACT
            var childContainer = container.GetNestedContainer();

            var instance0 = container.GetInstance<Func<ISingleton1>>().Invoke();
            var instance1 = childContainer.GetInstance<Func<ISingleton1>>().Invoke();
            var instance2 = childContainer.GetInstance<Func<Singleton1>>().Invoke();

            //ASSERT
            Assert.NotEqual(instance0, instance1);
            Assert.NotEqual(instance0, instance2);
            Assert.Equal(instance1, instance2);
        }
    }
}
