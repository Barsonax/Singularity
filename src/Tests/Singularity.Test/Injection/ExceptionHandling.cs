using System;
using System.Collections.Generic;
using Singularity.Exceptions;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class ExceptionHandling
    {
        [Fact]
        public void GetInstance_GetDependencyByConcreteType_Abstract_Throws()
        {
            var container = new Container();
            var e = Assert.Throws<DependencyNotFoundException>(() =>
            {
                var value = container.GetInstance<AbstractClass>();
            });
        }

        [Fact]
        public void GetInstance_GetDependencyByConcreteType_String_Throws()
        {
            var container = new Container();
            var e = Assert.Throws<DependencyNotFoundException>(() =>
            {
                var value = container.GetInstance<string>();
            });
        }

        [Fact]
        public void GetInstance_GetDependencyByConcreteType_Primitive_Throws()
        {
            var container = new Container();
            var e = Assert.Throws<DependencyNotFoundException>(() =>
            {
                var value = container.GetInstance(typeof(int));
            });
        }

        [Fact]
        public void GetInstance_GetDependencyByConcreteType_WithUnregisteredDependency_Throws()
        {
            var container = new Container();
            var e = Assert.Throws<DependencyResolveException>(() =>
            {
                var value = container.GetInstance<TestService12WithMixedConcreteDependency>();
            });
            Assert.IsType<DependencyNotFoundException>(e.InnerException);
        }

        [Fact]
        public void GetInstance_MissingDecoratorDependency_Throws()
        {
            try
            {
                var container = new Container(builder =>
                {
                    builder.Register<ITestService10, TestService10>();
                    builder.Decorate<ITestService10, TestService10_Decorator1>();
                });
            }
            catch (AggregateException e)
            {
                Assert.Equal(typeof(SingularityAggregateException), e.GetType());
                AggregateException aggregateException = e.Flatten();

                Assert.Single(aggregateException.InnerExceptions);
                Assert.Equal(typeof(DependencyNotFoundException), aggregateException.InnerExceptions[0].GetType());
                var dependencyNotFoundException = (DependencyNotFoundException)aggregateException.InnerExceptions[0];

                Assert.Equal(typeof(int).AssemblyQualifiedName, dependencyNotFoundException.Type);
            }
        }

        [Fact]
        public void GetInstance_MissingDependency_Throws()
        {
            var container = new Container();
            Assert.Throws<DependencyNotFoundException>(() =>
            {
                container.GetInstance<ITestService10>();
            });
        }

        [Fact]
        public void GetInstance_MissingInternalDependency_Throws()
        {
            try
            {
                var container = new Container(builder =>
                {
                    builder.Register<ITestService11, TestService11>();
                });
            }
            catch (AggregateException e)
            {
                Assert.Equal(typeof(SingularityAggregateException), e.GetType());
                AggregateException aggregateException = e.Flatten();

                Assert.Single(aggregateException.InnerExceptions);
                Assert.Equal(typeof(DependencyNotFoundException), aggregateException.InnerExceptions[0].GetType());
                var dependencyNotFoundException = (DependencyNotFoundException)aggregateException.InnerExceptions[0];

                Assert.Equal(typeof(ITestService10).AssemblyQualifiedName, dependencyNotFoundException.Type);
            }
        }

        [Fact]
        public void GetNestedContainer_OverrideInChild_OpenGeneric_Throws()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register(typeof(ISerializer<>), typeof(DefaultSerializer<>));
            });

            //ACT
            //ASSERT
            Assert.Throws<RegistrationAlreadyExistsException>(() =>
            {
                Container nestedContainer = container.GetNestedContainer(builder =>
                {
                    builder.Register<ISerializer<int>, IntSerializer>();
                });
            });
        }

        [Fact]
        public void DirectlyRegisteredEnumerable_Throws()
        {
            //ARRANGE
            Assert.Throws<EnumerableRegistrationException>(() =>
            {
                new Container(builder =>
                {
                    builder.Register<IEnumerable<IPlugin>>(c =>
                        c.Inject(() => new IPlugin[] {new Plugin1(), new Plugin2(), new Plugin3()}));
                });
            });
        }
    }
}
