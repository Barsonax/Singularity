using System;
using System.Collections.Generic;
using Singularity.Test.TestClasses;
using Xunit;

namespace Singularity.Test
{
    public class ContainerNoInternalDepedendencyTests
    {
        [Fact]
        public void GetInstanceFactory_NoInternalDependencies()
        {
            var config = new BindingConfig();
            config.For<ITestService10>().Inject<TestService10>();

            var container = new Container(config);

            var value = container.GetInstanceFactory<ITestService10>().Invoke();

            Assert.Equal(typeof(TestService10), value.GetType());
        }

        [Fact]
        public void GetInstance_NestedContainerNoInternalDependencies()
        {
            var config = new BindingConfig();
            config.For<ITestService10>().Inject<TestService10>();

            using (var container = new Container(config))
            {
                var nestedConfig = new BindingConfig();

                nestedConfig.For<ITestService10>().Inject<TestService10Variant>();
                using (var nestedContainer = container.GetNestedContainer(nestedConfig))
                {
                    var nestedValue = nestedContainer.GetInstance<ITestService10>();
                    Assert.Equal(typeof(TestService10Variant), nestedValue.GetType());
                }

                var value = container.GetInstance<ITestService10>();
                Assert.Equal(typeof(TestService10), value.GetType());
            }            
        }

        [Fact]
        public void GetInstance_NoInternalDependencies()
        {
            var config = new BindingConfig();
            config.For<ITestService10>().Inject<TestService10>();

            var container = new Container(config);

            var value = container.GetInstance<ITestService10>();
            Assert.Equal(typeof(TestService10), value.GetType());
        }

        [Fact]
        public void Inject_NoInternalDependencies()
        {
            var config = new BindingConfig();
            config.For<ITestService10>().Inject<TestService10>();

            var container = new Container(config);

            var instance = new MethodInjectionClass();
            container.MethodInject(instance);

            Assert.Equal(typeof(TestService10), instance.TestService10.GetType());
        }

        [Fact]
        public void InjectMultiple_NoInternalDependencies()
        {
            var config = new BindingConfig();
            config.For<ITestService10>().Inject<TestService10>();

            var container = new Container(config);

            var instances = new List<MethodInjectionClass>();
            for (var i = 0; i < 10; i++)
            {
                instances.Add(new MethodInjectionClass());
            }
            container.MethodInjectAll(instances);

            foreach (var instance in instances)
            {
                Assert.Equal(typeof(TestService10), instance.TestService10.GetType());
            }
        }

        [Fact]
        public void GetInstance_WithPerContainerLifetime()
        {
            var config = new BindingConfig();
            config.For<ITestService10>().Inject<TestService10>().With(Lifetime.PerContainer);

            var container = new Container(config);

            var value1 = container.GetInstance<ITestService10>();
            var value2 = container.GetInstance<ITestService10>();
            Assert.NotNull(value1);
            Assert.NotNull(value2);
            Assert.Equal(value1, value2);
        }

        [Fact]
        public void GetInstance_WithPerContainerLifetime_IsDisposed()
        {
            var config = new BindingConfig();
            config.For<IDisposable>().Inject<Disposable>().With(Lifetime.PerContainer);

            var container = new Container(config);

            var disposable = container.GetInstance<IDisposable>();
            Assert.NotNull(disposable);
            Assert.Equal(typeof(Disposable), disposable.GetType());

            var value = (Disposable)disposable;
            Assert.False(value.IsDisposed);
            container.Dispose();
            Assert.True(value.IsDisposed);
        }

        [Fact]
        public void GetInstance_WithPerCallLifetime()
        {
            var config = new BindingConfig();
            config.For<ITestService10>().Inject<TestService10>();

            var container = new Container(config);

            var value1 = container.GetInstance<ITestService10>();
            var value2 = container.GetInstance<ITestService10>();
            Assert.NotNull(value1);
            Assert.NotNull(value2);
            Assert.NotEqual(value1, value2);
        }
    }
}