using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
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
            ReadOnlyCollection<ReadonlyRegistration> registrations = config.GetDependencies().Registrations;

            //ASSERT
            Assert.Equal(3, registrations.Count);
            Assert.Equal(typeof(ITestService10), Assert.Single(registrations[0].DependencyTypes));
            Assert.Equal(typeof(ITestService11), Assert.Single(registrations[1].DependencyTypes));
            Assert.Equal(typeof(ITestService12), Assert.Single(registrations[2].DependencyTypes));
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
            ReadOnlyCollection<ReadonlyRegistration> registrations = config.GetDependencies().Registrations;

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
            }).With(Lifetime.PerContainer);

            //ACT
            ReadOnlyCollection<ReadonlyRegistration> registrations = config.GetDependencies().Registrations;

            //ASSERT
            ReadonlyRegistration registration = Assert.Single(registrations);
            Assert.Equal(typeof(Plugin1), registration.Bindings[0].Expression!.Type);
            Assert.Equal(typeof(Plugin2), registration.Bindings[1].Expression!.Type);
            Assert.Equal(typeof(Plugin3), registration.Bindings[2].Expression!.Type);
            Assert.True(registrations[0].Bindings.All(x => x.Lifetime == Lifetime.PerContainer));
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
            var readOnlyBindingConfig = config.GetDependencies();

            //ASSERT
            ReadonlyRegistration registration = Assert.Single(readOnlyBindingConfig.Registrations);
            Assert.Equal(typeof(Plugin1), registration.Bindings[0].Expression!.Type);
            Assert.Equal(typeof(Plugin2), registration.Bindings[1].Expression!.Type);
            Assert.Equal(typeof(Plugin3), registration.Bindings[2].Expression!.Type);

            ReadOnlyCollection<Expression> decorators = Assert.Single(readOnlyBindingConfig.Decorators.Values);

            Assert.Equal(new[] { typeof(PluginLogger1), typeof(PluginLogger2), typeof(PluginLogger3) }, decorators.Select(x => x.Type));
        }

        [Fact]
        public void Register_InvalidLifetime_StronglyTyped()
        {
            var config = new BindingConfig();
            Assert.Throws<InvalidEnumValue<Lifetime>>(() =>
            {
                config.Register<ITestService10, TestService10>().With((Lifetime)234234);
            });
        }

        [Fact]
        public void Register_InvalidLifetime_WeaklyTyped()
        {
            var config = new BindingConfig();
            Assert.Throws<InvalidEnumValue<Lifetime>>(() =>
            {
                config.Register(typeof(ITestService10), typeof(TestService10)).With((Lifetime)234234);
            });
        }

        [Fact]
        public void Register_InvalidDisposeBehavior_StronglyTyped()
        {
            var config = new BindingConfig();
            Assert.Throws<InvalidEnumValue<DisposeBehavior>>(() =>
            {
                config.Register<ITestService10, TestService10>().With((DisposeBehavior)234234);
            });
        }

        [Fact]
        public void Register_InvalidDisposeBehavior_WeaklyTyped()
        {
            var config = new BindingConfig();
            Assert.Throws<InvalidEnumValue<DisposeBehavior>>(() =>
            {
                config.Register(typeof(ITestService10), typeof(TestService10)).With((DisposeBehavior)234234);
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

            ReadonlyRegistration registration = Assert.Single(config.GetDependencies().Registrations);
            Assert.Equal(typeof(object), Assert.Single(registration.DependencyTypes));
            Assert.Equal(typeof(Func<object, object>), registration.Bindings.Single().Expression?.Type);
        }

        [Fact]
        public void ForInjectArity2()
        {
            var config = new BindingConfig();
            config.Register<object>().Inject<object, object, object>((obj0, obj1) => new object());

            ReadonlyRegistration registration = Assert.Single(config.GetDependencies().Registrations);
            Assert.Equal(typeof(object), Assert.Single(registration.DependencyTypes));
            Assert.Equal(typeof(Func<object, object, object>), registration.Bindings.Single().Expression?.Type);
        }

        [Fact]
        public void ForInjectArity3()
        {
            var config = new BindingConfig();
            config.Register<object>().Inject<object, object, object, object>((obj0, obj1, obj2) => new object());

            ReadonlyRegistration registration = Assert.Single(config.GetDependencies().Registrations);
            Assert.Equal(typeof(object), Assert.Single(registration.DependencyTypes));
            Assert.Equal(typeof(Func<object, object, object, object>), registration.Bindings.Single().Expression?.Type);
        }

        [Fact]
        public void ForInjectArity4()
        {
            var config = new BindingConfig();
            config.Register<object>().Inject<object, object, object, object, object>((obj0, obj1, obj2, obj3) => new object());

            ReadonlyRegistration registration = Assert.Single(config.GetDependencies().Registrations);
            Assert.Equal(typeof(object), Assert.Single(registration.DependencyTypes));
            Assert.Equal(typeof(Func<object, object, object, object, object>), registration.Bindings.Single().Expression?.Type);
        }

        [Fact]
        public void ForInjectArity5()
        {
            var config = new BindingConfig();
            config.Register<object>().Inject<object, object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4) => new object());

            ReadonlyRegistration registration = Assert.Single(config.GetDependencies().Registrations);
            Assert.Equal(typeof(object), Assert.Single(registration.DependencyTypes));
            Assert.Equal(typeof(Func<object, object, object, object, object, object>), registration.Bindings.Single().Expression?.Type);
        }

        [Fact]
        public void ForInjectArity6()
        {
            var config = new BindingConfig();
            config.Register<object>().Inject<object, object, object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4, obj5) => new object());

            ReadonlyRegistration registration = Assert.Single(config.GetDependencies().Registrations);
            Assert.Equal(typeof(object), Assert.Single(registration.DependencyTypes));
            Assert.Equal(typeof(Func<object, object, object, object, object, object, object>), registration.Bindings.Single().Expression?.Type);
        }

        [Fact]
        public void ForInjectArity7()
        {
            var config = new BindingConfig();
            config.Register<object>().Inject<object, object, object, object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4, obj5, obj6) => new object());

            ReadonlyRegistration registration = Assert.Single(config.GetDependencies().Registrations);
            Assert.Equal(typeof(object), Assert.Single(registration.DependencyTypes));
            Assert.Equal(typeof(Func<object, object, object, object, object, object, object, object>), registration.Bindings.Single().Expression?.Type);
        }

        [Fact]
        public void ForInjectArity8()
        {
            var config = new BindingConfig();
            config.Register<object>().Inject<object, object, object, object, object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4, obj5, obj6, obj7) => new object());

            ReadonlyRegistration registration = Assert.Single(config.GetDependencies().Registrations);
            Assert.Equal(typeof(object), Assert.Single(registration.DependencyTypes));
            Assert.Equal(typeof(Func<object, object, object, object, object, object, object, object, object>), registration.Bindings.Single().Expression?.Type);
        }
    }
}
