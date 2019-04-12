using System;
using System.Collections.ObjectModel;
using System.Linq;
using Singularity.Bindings;
using Singularity.Exceptions;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Bindings
{
    public class BindingConfigTests
    {
        [Fact]
        public void IncorrectBinding_Throws()
        {
            Assert.Throws<BindingConfigException>(() =>
            {
                var config = new BindingConfig();
                config.Register<ITestService10>();
                var container = new Container(config);
            });
        }

        [Fact]
        public void Decorate_NotAInterface_Throws()
        {
            var config = new BindingConfig();
            Assert.Throws<InterfaceExpectedException>(() =>
            {
                config.Decorate<TestService10, DecoratorWithNoInterface>();
            });
        }

        [Fact]
        public void GetDependencies_SingleRegistration_Enumerate()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<ITestService10, TestService10>();
            config.Register<ITestService11, TestService11>();
            config.Register<ITestService12, TestService12>();

            //ACT
            ReadOnlyCollection<ReadonlyRegistration> registrations = config.GetDependencies();

            //ASSERT
            Assert.Equal(3, registrations.Count);
            Assert.Contains(registrations, x => x.DependencyType == typeof(ITestService10));
            Assert.Contains(registrations, x => x.DependencyType == typeof(ITestService11));
            Assert.Contains(registrations, x => x.DependencyType == typeof(ITestService12));
        }

        [Fact]
        public void GetDependencies_MultiRegistration_Enumerate()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register(typeof(IPlugin), new[]
            {
                typeof(Plugin1),
                typeof(Plugin2),
                typeof(Plugin3),
            });

            //ACT
            ReadOnlyCollection<ReadonlyRegistration> registrations = config.GetDependencies();

            //ASSERT
            ReadonlyRegistration registration = Assert.Single(registrations);
            Assert.Equal(typeof(Plugin1), registration.Bindings[0].Expression!.Type);
            Assert.Equal(typeof(Plugin2), registration.Bindings[1].Expression!.Type);
            Assert.Equal(typeof(Plugin3), registration.Bindings[2].Expression!.Type);
        }

        [Fact]
        public void GetDependencies_MultiRegistrationWithLifetime_Enumerate()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register(typeof(IPlugin), new[]
            {
                typeof(Plugin1),
                typeof(Plugin2),
                typeof(Plugin3),
            }).With(CreationMode.PerContainer);

            //ACT
            ReadOnlyCollection<ReadonlyRegistration> registrations = config.GetDependencies();

            //ASSERT
            ReadonlyRegistration registration = Assert.Single(registrations);
            Assert.Equal(typeof(Plugin1), registration.Bindings[0].Expression!.Type);
            Assert.Equal(typeof(Plugin2), registration.Bindings[1].Expression!.Type);
            Assert.Equal(typeof(Plugin3), registration.Bindings[2].Expression!.Type);
            Assert.True(registrations[0].Bindings.All(x => x.CreationMode == CreationMode.PerContainer));
        }

        [Fact]
        public void GetDependencies_MultiDecoratorRegistration_Enumerate()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IPlugin, Plugin1>();
            config.Register<IPlugin, Plugin2>();
            config.Register<IPlugin, Plugin3>();

            config.Decorate(typeof(IPlugin), new[]
            {
                typeof(PluginLogger1),
                typeof(PluginLogger2),
                typeof(PluginLogger3),
            });

            //ACT
            ReadOnlyCollection<ReadonlyRegistration> registrations = config.GetDependencies();

            //ASSERT
            ReadonlyRegistration registration = Assert.Single(registrations);
            Assert.Equal(typeof(Plugin1), registration.Bindings[0].Expression!.Type);
            Assert.Equal(typeof(Plugin2), registration.Bindings[1].Expression!.Type);
            Assert.Equal(typeof(Plugin3), registration.Bindings[2].Expression!.Type);

            Assert.Equal(new[] { typeof(PluginLogger1), typeof(PluginLogger2), typeof(PluginLogger3) }, registration.Decorators.Select(x => x.Type));
        }

        [Fact]
        public void Register_InvalidLifetime_StronglyTyped()
        {
            var config = new BindingConfig();
            Assert.Throws<InvalidLifetimeException>(() =>
            {
                config.Register<ITestService10, TestService10>().With((CreationMode)234234);
            });
        }

        [Fact]
        public void Register_InvalidLifetime_WeaklyTyped()
        {
            var config = new BindingConfig();
            Assert.Throws<InvalidLifetimeException>(() =>
            {
                config.Register(typeof(ITestService10), typeof(TestService10)).With((CreationMode)234234);
            });
        }

        [Fact]
        public void Register_TypeNotAssignable()
        {
            var config = new BindingConfig();
            Assert.Throws<TypeNotAssignableException>(() =>
            {
                config.Register(typeof(ITestService10), typeof(TestService11));
            });
        }

        [Fact]
        public void Decorate_InvalidConstructorArguments_WeaklyTyped_Throws()
        {
            var config = new BindingConfig();
            Assert.Throws<InvalidExpressionArgumentsException>(() =>
            {
                config.Decorate(typeof(ITestService10), typeof(DecoratorWrongConstructorArguments));
            });
        }

        [Fact]
        public void Decorate_InvalidConstructorArguments_StronglyTyped_Throws()
        {
            var config = new BindingConfig();
            Assert.Throws<InvalidExpressionArgumentsException>(() =>
            {
                config.Decorate<ITestService10, DecoratorWrongConstructorArguments>();
            });
        }

        [Fact]
        public void ForInjectArity1()
        {
            var config = new BindingConfig();
            config.Register<object>().Inject<object, object>((obj0) => new object());

            ReadonlyRegistration registration = config.GetDependencies().Single();
            Assert.Equal(typeof(object), registration.DependencyType);
            Assert.Equal(typeof(Func<object, object>), registration.Bindings.Single().Expression?.Type);
        }

        [Fact]
        public void ForInjectArity2()
        {
            var config = new BindingConfig();
            config.Register<object>().Inject<object, object, object>((obj0, obj1) => new object());

            ReadonlyRegistration registration = config.GetDependencies().Single();
            Assert.Equal(typeof(object), registration.DependencyType);
            Assert.Equal(typeof(Func<object, object, object>), registration.Bindings.Single().Expression?.Type);
        }

        [Fact]
        public void ForInjectArity3()
        {
            var config = new BindingConfig();
            config.Register<object>().Inject<object, object, object, object>((obj0, obj1, obj2) => new object());

            ReadonlyRegistration registration = config.GetDependencies().Single();
            Assert.Equal(typeof(object), registration.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object>), registration.Bindings.Single().Expression?.Type);
        }

        [Fact]
        public void ForInjectArity4()
        {
            var config = new BindingConfig();
            config.Register<object>().Inject<object, object, object, object, object>((obj0, obj1, obj2, obj3) => new object());

            ReadonlyRegistration registration = config.GetDependencies().Single();
            Assert.Equal(typeof(object), registration.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object, object>), registration.Bindings.Single().Expression?.Type);
        }

        [Fact]
        public void ForInjectArity5()
        {
            var config = new BindingConfig();
            config.Register<object>().Inject<object, object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4) => new object());

            ReadonlyRegistration registration = config.GetDependencies().Single();
            Assert.Equal(typeof(object), registration.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object, object, object>), registration.Bindings.Single().Expression?.Type);
        }

        [Fact]
        public void ForInjectArity6()
        {
            var config = new BindingConfig();
            config.Register<object>().Inject<object, object, object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4, obj5) => new object());

            ReadonlyRegistration registration = config.GetDependencies().Single();
            Assert.Equal(typeof(object), registration.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object, object, object, object>), registration.Bindings.Single().Expression?.Type);
        }

        [Fact]
        public void ForInjectArity7()
        {
            var config = new BindingConfig();
            config.Register<object>().Inject<object, object, object, object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4, obj5, obj6) => new object());

            ReadonlyRegistration registration = config.GetDependencies().Single();
            Assert.Equal(typeof(object), registration.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object, object, object, object, object>), registration.Bindings.Single().Expression?.Type);
        }

        [Fact]
        public void ForInjectArity8()
        {
            var config = new BindingConfig();
            config.Register<object>().Inject<object, object, object, object, object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4, obj5, obj6, obj7) => new object());

            ReadonlyRegistration registration = config.GetDependencies().Single();
            Assert.Equal(typeof(object), registration.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object, object, object, object, object, object>), registration.Bindings.Single().Expression?.Type);
        }
    }
}
