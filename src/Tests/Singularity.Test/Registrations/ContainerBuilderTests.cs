using System;
using System.Linq;
using System.Linq.Expressions;
using Singularity.Collections;
using Singularity.Exceptions;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Registrations
{
    public class BindingConfigTests
    {
        [Fact]
        public void Decorate_NotAInterface_Throws()
        {
            Assert.Throws<InterfaceExpectedException>(() =>
            {
                new Container(builder =>
                {
                    builder.Decorate<TestService10, DecoratorWithNoInterface>();
                });
            });
        }

        [Fact]
        public void GetDependencies_SingleRegistration_Enumerate()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ITestService10, TestService10>();
                builder.Register<ITestService11, TestService11>();
                builder.Register<ITestService12, TestService12>();
            });

            //ACT
            Registration[] registrations = container.Registrations.Registrations.Values.ToArray();

            //ASSERT
            Assert.Equal(3, registrations.Length);
            Assert.Equal(typeof(ITestService10), registrations[0].DependencyType);
            Assert.Equal(typeof(ITestService11), registrations[1].DependencyType);
            Assert.Equal(typeof(ITestService12), registrations[2].DependencyType);
        }

        [Fact]
        public void GetDependencies_MultiRegistration_Enumerate()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register(typeof(IPlugin), new[]
                {
                    typeof(Plugin1),
                    typeof(Plugin2),
                    typeof(Plugin3),
                });
            });

            //ACT
            Registration[] registrations = container.Registrations.Registrations.Values.ToArray();

            //ASSERT
            ServiceBinding[] serviceBindings = Assert.Single(registrations).Bindings.ToArray();
            Assert.Equal(typeof(Plugin1), serviceBindings[0].Expression!.Type);
            Assert.Equal(typeof(Plugin2), serviceBindings[1].Expression!.Type);
            Assert.Equal(typeof(Plugin3), serviceBindings[2].Expression!.Type);
        }

        [Fact]
        public void GetDependencies_MultiRegistrationWithLifetime_Enumerate()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register(typeof(IPlugin), new[]
                {
                    typeof(Plugin1),
                    typeof(Plugin2),
                    typeof(Plugin3),
                }, c => c
                    .With(Lifetimes.PerContainer));
            });

            //ACT
            Registration[] registrations = container.Registrations.Registrations.Values.ToArray();

            //ASSERT
            ServiceBinding[] serviceBindings = Assert.Single(registrations).Bindings.ToArray();
            Assert.Equal(typeof(Plugin1), serviceBindings[0].Expression!.Type);
            Assert.Equal(typeof(Plugin2), serviceBindings[1].Expression!.Type);
            Assert.Equal(typeof(Plugin3), serviceBindings[2].Expression!.Type);
            Assert.True(registrations[0].Bindings.All(x => x.Lifetime == Lifetimes.PerContainer));
        }

        [Fact]
        public void GetDependencies_MultiDecoratorRegistration_Enumerate()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IPlugin, Plugin1>();
                builder.Register<IPlugin, Plugin2>();
                builder.Register<IPlugin, Plugin3>();

                builder.Decorate(typeof(IPlugin), new[]
                {
                    typeof(PluginLogger1),
                    typeof(PluginLogger2),
                    typeof(PluginLogger3),
                });
            });

            //ACT
            RegistrationStore readOnlyBindingConfig = container.Registrations;

            //ASSERT
            ServiceBinding[] serviceBindings = Assert.Single(readOnlyBindingConfig.Registrations).Value.Bindings.ToArray();
            Assert.Equal(typeof(Plugin1), serviceBindings[0].Expression!.Type);
            Assert.Equal(typeof(Plugin2), serviceBindings[1].Expression!.Type);
            Assert.Equal(typeof(Plugin3), serviceBindings[2].Expression!.Type);

            ArrayList<Expression> decorators = Assert.Single(readOnlyBindingConfig.Decorators.Values);

            Assert.Equal(new[] { typeof(PluginLogger1), typeof(PluginLogger2), typeof(PluginLogger3) }, decorators.Select(x => x.Type));
        }

        [Fact]
        public void Register_InvalidDisposeBehavior_StronglyTyped()
        {
            Assert.Throws<InvalidEnumValueException<ServiceAutoDispose>>(() =>
            {
                new Container(builder =>
                {
                    builder.Register<ITestService10, TestService10>(c => c
                        .With((ServiceAutoDispose)234234));
                });
            });
        }

        [Fact]
        public void Register_InvalidDisposeBehavior_WeaklyTyped()
        {
            Assert.Throws<InvalidEnumValueException<ServiceAutoDispose>>(() =>
            {
                new Container(builder =>
                {
                    builder.Register(typeof(ITestService10), typeof(TestService10), c => c
                        .With((ServiceAutoDispose)234234));
                });
            });
        }

        [Fact]
        public void Register_TypeNotAssignable()
        {
            Assert.Throws<TypeNotAssignableException>(() =>
            {
                new Container(builder =>
                {
                    builder.Register(typeof(ITestService10), typeof(TestService11));
                });
            });
        }

        [Fact]
        public void Decorate_InvalidConstructorArguments_WeaklyTyped_Throws()
        {
            Assert.Throws<InvalidExpressionArgumentsException>(() =>
            {
                new Container(builder =>
                {
                    builder.Decorate(typeof(ITestService10), typeof(DecoratorWrongConstructorArguments));
                });
            });
        }

        [Fact]
        public void Decorate_InvalidConstructorArguments_StronglyTyped_Throws()
        {
            Assert.Throws<InvalidExpressionArgumentsException>(() =>
            {
                new Container(builder =>
                {
                    builder.Decorate<ITestService10, DecoratorWrongConstructorArguments>();
                });
            });
        }

        [Fact]
        public void Inject_WeaklyTyped()
        {
            var container = new Container(builder =>
            {
                builder.Register(typeof(object),c => c.Inject(Expression.Constant(new object())));
            });

            Registration registration = Assert.Single(container.Registrations.Registrations.Values);
            Assert.Equal(typeof(object), registration.DependencyType);
            Assert.Equal(typeof(object), registration.Bindings.Single().Expression?.Type);
        }

        [Fact]
        public void Inject_StronglyTyped_Arity1()
        {
            var container = new Container(builder =>
            {
                builder.Register<object>(c => c.Inject<object>(obj0 => new object()));
            });

            Registration registration = Assert.Single(container.Registrations.Registrations.Values);
            Assert.Equal(typeof(object), registration.DependencyType);
            Assert.Equal(typeof(Func<object, object>), registration.Bindings.Single().Expression?.Type);
        }

        [Fact]
        public void Inject_StronglyTyped_Arity2()
        {
            var container = new Container(builder =>
            {
                builder.Register<object>(c => c.Inject<object, object>((obj0, obj1) => new object()));
            });

            Registration registration = Assert.Single(container.Registrations.Registrations.Values);
            Assert.Equal(typeof(object), registration.DependencyType);
            Assert.Equal(typeof(Func<object, object, object>), registration.Bindings.Single().Expression?.Type);
        }

        [Fact]
        public void Inject_StronglyTyped_Arity3()
        {
            var container = new Container(builder =>
            {
                builder.Register<object>(c => c.Inject<object, object, object>((obj0, obj1, obj2) => new object()));
            });

            Registration registration = Assert.Single(container.Registrations.Registrations.Values);
            Assert.Equal(typeof(object), registration.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object>), registration.Bindings.Single().Expression?.Type);
        }

        [Fact]
        public void Inject_StronglyTyped_Arity4()
        {
            var container = new Container(builder =>
            {
                builder.Register<object>(c => c.Inject<object, object, object, object>((obj0, obj1, obj2, obj3) => new object()));
            });

            Registration registration = Assert.Single(container.Registrations.Registrations.Values);
            Assert.Equal(typeof(object), registration.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object, object>), registration.Bindings.Single().Expression?.Type);
        }

        [Fact]
        public void Inject_StronglyTyped_Arity5()
        {
            var container = new Container(builder =>
            {
                builder.Register<object>(c => c.Inject<object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4) => new object()));
            });

            Registration registration = Assert.Single(container.Registrations.Registrations.Values);
            Assert.Equal(typeof(object), registration.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object, object, object>), registration.Bindings.Single().Expression?.Type);
        }

        [Fact]
        public void Inject_StronglyTyped_Arity6()
        {
            var container = new Container(builder =>
            {
                builder.Register<object>(c => c.Inject<object, object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4, obj5) => new object()));
            });

            Registration registration = Assert.Single(container.Registrations.Registrations.Values);
            Assert.Equal(typeof(object), registration.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object, object, object, object>), registration.Bindings.Single().Expression?.Type);
        }

        [Fact]
        public void Inject_StronglyTyped_Arity7()
        {
            var container = new Container(builder =>
            {
                builder.Register<object>(c => c.Inject<object, object, object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4, obj5, obj6) => new object()));
            });

            Registration registration = Assert.Single(container.Registrations.Registrations.Values);
            Assert.Equal(typeof(object), registration.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object, object, object, object, object>), registration.Bindings.Single().Expression?.Type);
        }

        [Fact]
        public void Inject_StronglyTyped_Arity8()
        {
            var container = new Container(builder =>
            {
                builder.Register<object>(c => c.Inject<object, object, object, object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4, obj5, obj6, obj7) => new object()));
            });

            Registration registration = Assert.Single(container.Registrations.Registrations.Values);
            Assert.Equal(typeof(object), registration.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object, object, object, object, object, object>), registration.Bindings.Single().Expression?.Type);
        }
    }
}
