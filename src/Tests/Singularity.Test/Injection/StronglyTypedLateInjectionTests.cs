using System;
using System.Collections.Generic;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class StronglyTypedLateInjectionTests
    {
        [Fact]
        public void MethodInject_InjectsCorrectDependencies()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ITestService10, TestService10>();
                builder.LateInject<MethodInjectionClass>(c => c
                    .UseMethod(nameof(MethodInjectionClass.Inject)));
            });
            var instance = new MethodInjectionClass();

            //ACT
            container.LateInject(instance);

            //ASSERT
            Assert.IsType<TestService10>(instance.TestService10);
        }

        [Fact]
        public void PropertyInject_Name_InjectsCorrectDependencies()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ITestService10, TestService10>();
                builder.LateInject<MethodInjectionClass>(c => c
                    .UseProperty(nameof(MethodInjectionClass.TestService10)));
            });
            var instance = new MethodInjectionClass();

            //ACT
            container.LateInject(instance);

            //ASSERT
            Assert.IsType<TestService10>(instance.TestService10);
        }

        [Fact]
        public void FieldInject_Name_InjectsCorrectDependencies()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ITestService10, TestService10>();
                builder.LateInject<MethodInjectionClass>(c => c
                    .UseField(nameof(MethodInjectionClass.TestService10Field)));
            });
            var instance = new MethodInjectionClass();

            //ACT
            container.LateInject(instance);

            //ASSERT
            Assert.IsType<TestService10>(instance.TestService10Field);
        }

        [Fact]
        public void MemberInject_Expression_Property_InjectsCorrectDependencies()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ITestService10, TestService10>();
                builder.LateInject<MethodInjectionClass>(c => c
                    .UseMember(o => o.TestService10));
            });
            var instance = new MethodInjectionClass();

            //ACT
            container.LateInject(instance);

            //ASSERT
            Assert.IsType<TestService10>(instance.TestService10);
        }

        [Fact]
        public void FieldInject_Expression_Field_InjectsCorrectDependencies()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ITestService10, TestService10>();
                builder.LateInject<MethodInjectionClass>(c => c
                    .UseMember(o => o.TestService10Field));
            });
            var instance = new MethodInjectionClass();

            //ACT
            container.LateInject(instance);

            //ASSERT
            Assert.IsType<TestService10>(instance.TestService10Field);
        }

        [Fact]
        public void FieldInject_Expression_Method_Throws()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ITestService10, TestService10>();

                //ACT
                //ASSERT
                Assert.Throws<NotSupportedException>(() =>
                {
                    builder.LateInject<MethodInjectionClass>(c => c
                        .UseMember(o => o.FakeMethodWithReturn()));
                });

            });
        }

        [Fact]
        public void MethodInjectAll_InjectsCorrectDependencies()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ITestService10, TestService10>();
                builder.LateInject<MethodInjectionClass>(c => c
                    .UseMethod(nameof(MethodInjectionClass.Inject)));
            });

            var instances = new List<MethodInjectionClass>();
            for (var i = 0; i < 10; i++)
            {
                instances.Add(new MethodInjectionClass());
            }

            //ACT
            container.LateInjectAll(instances);

            //ASSERT
            foreach (MethodInjectionClass instance in instances)
            {
                Assert.IsType<TestService10>(instance.TestService10);
            }
        }

        [Fact]
        public void MethodInjectAll_Scoped_InjectsCorrectDependencies()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ITestService10, TestService10>();
                builder.LateInject<MethodInjectionClass>(c => c
                    .UseMethod(nameof(MethodInjectionClass.Inject)));
            });

            var instances = new List<MethodInjectionClass>();
            for (var i = 0; i < 10; i++)
            {
                instances.Add(new MethodInjectionClass());
            }

            //ACT
            Scoped scope = container.BeginScope();
            scope.LateInjectAll(instances);

            //ASSERT
            foreach (MethodInjectionClass instance in instances)
            {
                Assert.IsType<TestService10>(instance.TestService10);
            }
        }

        [Fact]
        public void MethodInject_RegistrationWithNoInjections()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ITestService10, TestService10>();
                builder.LateInject<MethodInjectionClass>(c =>
                {
                    //Do nothing
                });
            });

            var instance = new MethodInjectionClass();

            //ACT
            container.LateInject(instance);

            //ASSERT
            Assert.Null(instance.TestService10);
        }

        [Fact]
        public void MethodInject_NoInjectionsRegistered()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ITestService10, TestService10>();
            });

            var instance = new MethodInjectionClass();

            //ACT
            container.LateInject(instance);

            //ASSERT
            Assert.Null(instance.TestService10);
        }
    }
}
