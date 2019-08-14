using System.Collections.Generic;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class WeaklyTypedLateInjectionTests
    {
        [Fact]
        public void MethodInject_InjectsCorrectDependencies()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ITestService10, TestService10>();
                builder.LateInject(typeof(MethodInjectionClass), c => c
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
                builder.LateInject(typeof(MethodInjectionClass), c => c
                    .UseProperty(nameof(MethodInjectionClass.TestService10)));
            });
            var instance = new MethodInjectionClass();

            //ACT
            container.LateInject(instance);

            //ASSERT
            Assert.IsType<TestService10>(instance.TestService10);
        }

        [Fact]
        public void MethodInject_Scoped_InjectsCorrectDependencies()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ITestService10, TestService10>();
                builder.LateInject(typeof(MethodInjectionClass), c => c
                    .UseMethod(nameof(MethodInjectionClass.Inject)));
            });
            var instance = new MethodInjectionClass();

            //ACT
            Scoped scope = container.BeginScope();
            scope.LateInject(instance);

            //ASSERT
            Assert.IsType<TestService10>(instance.TestService10);
        }

        [Fact]
        public void MethodInjectAll_InjectsCorrectDependencies()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ITestService10, TestService10>();
                builder.LateInject(typeof(MethodInjectionClass), c => c
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
    }
}
