using System;
using System.Collections.Generic;
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
                new ContainerBuilder(cb =>
                {
                    cb.Decorate<TestService10, DecoratorWithNoInterface>();
                });
            });
        }

        [Fact]
        public void GetDependencies_SingleRegistration_Enumerate()
        {
            //ARRANGE
            var builder = new ContainerBuilder(cb =>
            {
                cb.Register<ITestService10, TestService10>();
                cb.Register<ITestService11, TestService11>();
                cb.Register<ITestService12, TestService12>();
            });

            //ACT
            var registrations = builder.Registrations.Registrations.ToArray();

            //ASSERT
            Assert.Equal(6, registrations.Length);
            Assert.Equal(typeof(ITestService10), registrations[0].Key);
            Assert.Equal(typeof(TestService10), registrations[1].Key);
            Assert.Equal(typeof(ITestService11), registrations[2].Key);
            Assert.Equal(typeof(TestService11), registrations[3].Key);
            Assert.Equal(typeof(ITestService12), registrations[4].Key);
            Assert.Equal(typeof(TestService12), registrations[5].Key);
        }

        [Fact]
        public void GetDependencies_MultiRegistration_Enumerate()
        {
            //ARRANGE
            var builder = new ContainerBuilder(cb =>
            {
                //ACT
                cb.Register(typeof(IPlugin), new[]
                {
                    typeof(Plugin1),
                    typeof(Plugin2),
                    typeof(Plugin3),
                });
            });

            //ASSERT
            var registrations = builder.Registrations.Registrations;

            Assert.Equal(4, registrations.Count());

            var multiServiceBinding = registrations[typeof(IPlugin)];

            Assert.Collection(multiServiceBinding, 
                e => Assert.Equal(typeof(Plugin3), e.Expression?.Type),
                e => Assert.Equal(typeof(Plugin2), e.Expression?.Type),
                e => Assert.Equal(typeof(Plugin1), e.Expression?.Type));

            var plugin1Binding = registrations[typeof(Plugin1)];
            Assert.Single(plugin1Binding);

            var plugin2Binding = registrations[typeof(Plugin2)];
            Assert.Single(plugin2Binding);

            var plugin3Binding = registrations[typeof(Plugin3)];
            Assert.Single(plugin3Binding);
        }

        [Fact]
        public void GetDependencies_MultiRegistrationWithLifetime_Enumerate()
        {
            //ARRANGE
            var builder = new ContainerBuilder(cb =>
            {
                //ACT
                cb.Register(typeof(IPlugin), new[]
                {
                    typeof(Plugin1),
                    typeof(Plugin2),
                    typeof(Plugin3),
                }, c => c
                    .With(Lifetimes.PerContainer));
            });


            //ASSERT
            var registrations = builder.Registrations.Registrations;

            Assert.Equal(4, registrations.Count());

            var multiServiceBinding = registrations[typeof(IPlugin)];

            Assert.Collection(multiServiceBinding,
                e => Assert.Equal(typeof(Plugin3), e.Expression?.Type),
                e => Assert.Equal(typeof(Plugin2), e.Expression?.Type),
                e => Assert.Equal(typeof(Plugin1), e.Expression?.Type));

            var plugin1Binding = registrations[typeof(Plugin1)];
            Assert.Single(plugin1Binding);

            var plugin2Binding = registrations[typeof(Plugin2)];
            Assert.Single(plugin2Binding);

            var plugin3Binding = registrations[typeof(Plugin3)];
            Assert.Single(plugin3Binding);

            Assert.True(registrations.Values.All(x => x.All(y => y.Lifetime == Lifetimes.PerContainer)));
        }

        [Fact]
        public void GetDependencies_MultiDecoratorRegistration_Enumerate()
        {
            //ARRANGE
            var builder = new ContainerBuilder(cb =>
            {
                cb.Register<IPlugin, Plugin1>();
                cb.Register<IPlugin, Plugin2>();
                cb.Register<IPlugin, Plugin3>();

                cb.Decorate(typeof(IPlugin), new[]
                {
                    typeof(PluginLogger1),
                    typeof(PluginLogger2),
                    typeof(PluginLogger3),
                });
            });

            //ACT
            RegistrationStore readOnlyBindingConfig = builder.Registrations;

            //ASSERT

            Assert.Equal(4, readOnlyBindingConfig.Registrations.Count());

            var multiServiceBinding = readOnlyBindingConfig.Registrations[typeof(IPlugin)];

            Assert.Collection(multiServiceBinding,
                e => Assert.Equal(typeof(Plugin3), e.Expression?.Type),
                e => Assert.Equal(typeof(Plugin2), e.Expression?.Type),
                e => Assert.Equal(typeof(Plugin1), e.Expression?.Type));

            var plugin1Binding = readOnlyBindingConfig.Registrations[typeof(Plugin1)];
            Assert.Single(plugin1Binding);

            var plugin2Binding = readOnlyBindingConfig.Registrations[typeof(Plugin2)];
            Assert.Single(plugin2Binding);

            var plugin3Binding = readOnlyBindingConfig.Registrations[typeof(Plugin3)];
            Assert.Single(plugin3Binding);

            ArrayList<Expression> decorators = Assert.Single(readOnlyBindingConfig.Decorators.Values);

            Assert.Equal(new[] { typeof(PluginLogger1), typeof(PluginLogger2), typeof(PluginLogger3) }, decorators.Select(x => x.Type));
        }

        [Fact]
        public void Register_InvalidDisposeBehavior_StronglyTyped()
        {
            Assert.Throws<InvalidEnumValueException<ServiceAutoDispose>>(() =>
            {
                new ContainerBuilder(cb =>
                {
                    cb.Register<ITestService10, TestService10>(c => c
                        .With((ServiceAutoDispose)234234));
                });
            });
        }

        [Fact]
        public void Register_InvalidDisposeBehavior_WeaklyTyped()
        {
            Assert.Throws<InvalidEnumValueException<ServiceAutoDispose>>(() =>
            {
                new ContainerBuilder(cb =>
                {
                    cb.Register(typeof(ITestService10), typeof(TestService10), c => c
                        .With((ServiceAutoDispose)234234));
                });
            });
        }

        [Fact]
        public void Register_TypeNotAssignable()
        {
            Assert.Throws<TypeNotAssignableException>(() =>
            {
                new ContainerBuilder(cb =>
                {
                    cb.Register(typeof(ITestService10), typeof(TestService11));
                });
            });
        }

        [Fact]
        public void Decorate_InvalidConstructorArguments_WeaklyTyped_Throws()
        {
            Assert.Throws<InvalidExpressionArgumentsException>(() =>
            {
                new ContainerBuilder(cb =>
                {
                    cb.Decorate(typeof(ITestService10), typeof(DecoratorWrongConstructorArguments));
                });
            });
        }

        [Fact]
        public void Decorate_InvalidConstructorArguments_StronglyTyped_Throws()
        {
            Assert.Throws<InvalidExpressionArgumentsException>(() =>
            {
                new ContainerBuilder(cb =>
                {
                    cb.Decorate<ITestService10, DecoratorWrongConstructorArguments>();
                });
            });
        }

        [Fact]
        public void Inject_WeaklyTyped()
        {
            var builder = new ContainerBuilder(cb =>
            {
                cb.Register(typeof(object), c => c.Inject(Expression.Constant(new object())));
            });

            KeyValuePair<Type, SinglyLinkedListNode<ServiceBinding>> registration = Assert.Single(builder.Registrations.Registrations);
            Assert.Equal(typeof(object), registration.Key);
            Assert.Equal(typeof(object), registration.Value.Single().Expression?.Type);
        }

        [Fact]
        public void Inject_StronglyTyped_Arity1()
        {
            var builder = new ContainerBuilder(cb =>
            {
                cb.Register<object>(c => c.Inject<object>(obj0 => new ArgumentArity1TestClass(obj0)));
            });

            KeyValuePair<Type, SinglyLinkedListNode<ServiceBinding>> registration = Assert.Single(builder.Registrations.Registrations);
            Assert.Equal(typeof(object), registration.Key);
            var binding = Assert.Single(registration.Value);
            var expression = Assert.IsType<NewExpression>(binding.Expression);
            Assert.Equal(typeof(ArgumentArity1TestClass), expression.Type);
            Assert.Single(expression.Arguments);
        }

        [Fact]
        public void Inject_StronglyTyped_Arity2()
        {
            var builder = new ContainerBuilder(cb =>
            {
                cb.Register<object>(c => c.Inject<object, object>((obj0, obj1) => new ArgumentArity2TestClass(obj0, obj1)));
            });

            KeyValuePair<Type, SinglyLinkedListNode<ServiceBinding>> registration = Assert.Single(builder.Registrations.Registrations);
            Assert.Equal(typeof(object), registration.Key);
            var binding = Assert.Single(registration.Value);
            var expression = Assert.IsType<NewExpression>(binding.Expression);
            Assert.Equal(typeof(ArgumentArity2TestClass), expression.Type);
            Assert.Equal(2, expression.Arguments.Count);
        }

        [Fact]
        public void Inject_StronglyTyped_Arity3()
        {
            var builder = new ContainerBuilder(cb =>
            {
                cb.Register<object>(c => c.Inject<object, object, object>((obj0, obj1, obj2) => new ArgumentArity3TestClass(obj0, obj1, obj2)));
            });

            KeyValuePair<Type, SinglyLinkedListNode<ServiceBinding>> registration = Assert.Single(builder.Registrations.Registrations);
            Assert.Equal(typeof(object), registration.Key);
            var binding = Assert.Single(registration.Value);
            var expression = Assert.IsType<NewExpression>(binding.Expression);
            Assert.Equal(typeof(ArgumentArity3TestClass), expression.Type);
            Assert.Equal(3, expression.Arguments.Count);
        }

        [Fact]
        public void Inject_StronglyTyped_Arity4()
        {
            var builder = new ContainerBuilder(cb =>
            {
                cb.Register<object>(c => c.Inject<object, object, object, object>((obj0, obj1, obj2, obj3) => new ArgumentArity4TestClass(obj0, obj1, obj2, obj3)));
            });

            KeyValuePair<Type, SinglyLinkedListNode<ServiceBinding>> registration = Assert.Single(builder.Registrations.Registrations);
            Assert.Equal(typeof(object), registration.Key);
            var binding = Assert.Single(registration.Value);
            var expression = Assert.IsType<NewExpression>(binding.Expression);
            Assert.Equal(typeof(ArgumentArity4TestClass), expression.Type);
            Assert.Equal(4, expression.Arguments.Count);
        }

        [Fact]
        public void Inject_StronglyTyped_Arity5()
        {
            var builder = new ContainerBuilder(cb =>
            {
                cb.Register<object>(c => c.Inject<object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4) => new ArgumentArity5TestClass(obj0, obj1, obj2, obj3, obj4)));
            });

            KeyValuePair<Type, SinglyLinkedListNode<ServiceBinding>> registration = Assert.Single(builder.Registrations.Registrations);
            Assert.Equal(typeof(object), registration.Key);
            var binding = Assert.Single(registration.Value);
            var expression = Assert.IsType<NewExpression>(binding.Expression);
            Assert.Equal(typeof(ArgumentArity5TestClass), expression.Type);
            Assert.Equal(5, expression.Arguments.Count);
        }

        [Fact]
        public void Inject_StronglyTyped_Arity6()
        {
            var builder = new ContainerBuilder(cb =>
            {
                cb.Register<object>(c => c.Inject<object, object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4, obj5) => new ArgumentArity6TestClass(obj0, obj1, obj2, obj3, obj4, obj5)));
            });

            KeyValuePair<Type, SinglyLinkedListNode<ServiceBinding>> registration = Assert.Single(builder.Registrations.Registrations);
            Assert.Equal(typeof(object), registration.Key);
            var binding = Assert.Single(registration.Value);
            var expression = Assert.IsType<NewExpression>(binding.Expression);
            Assert.Equal(typeof(ArgumentArity6TestClass), expression.Type);
            Assert.Equal(6, expression.Arguments.Count);
        }

        [Fact]
        public void Inject_StronglyTyped_Arity7()
        {
            var builder = new ContainerBuilder(cb =>
            {
                cb.Register<object>(c => c.Inject<object, object, object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4, obj5, obj6) => new ArgumentArity7TestClass(obj0, obj1, obj2, obj3, obj4, obj5, obj6)));
            });

            KeyValuePair<Type, SinglyLinkedListNode<ServiceBinding>> registration = Assert.Single(builder.Registrations.Registrations);
            Assert.Equal(typeof(object), registration.Key);
            var binding = Assert.Single(registration.Value);
            var expression = Assert.IsType<NewExpression>(binding.Expression);
            Assert.Equal(typeof(ArgumentArity7TestClass), expression.Type);
            Assert.Equal(7, expression.Arguments.Count);
        }

        [Fact]
        public void Inject_StronglyTyped_Arity8()
        {
            var builder = new ContainerBuilder(cb =>
            {
                cb.Register<object>(c => c.Inject<object, object, object, object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4, obj5, obj6, obj7) => new ArgumentArity8TestClass(obj0, obj1, obj2, obj3, obj4, obj5, obj6, obj7)));
            });

            KeyValuePair<Type, SinglyLinkedListNode<ServiceBinding>> registration = Assert.Single(builder.Registrations.Registrations);
            Assert.Equal(typeof(object), registration.Key);
            var binding = Assert.Single(registration.Value);
            var expression = Assert.IsType<NewExpression>(binding.Expression);
            Assert.Equal(typeof(ArgumentArity8TestClass), expression.Type);
            Assert.Equal(8, expression.Arguments.Count);
        }
    }
}
