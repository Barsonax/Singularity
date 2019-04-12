using System;
using System.Collections.Generic;
using System.Linq;
using Singularity.Exceptions;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class ExceptionHandling
    {
        [Fact]
        public void GetInstance_GetDependencyByConcreteType_WithMixedConcreteDependency_2Deep_ReturnsCorrectDependency()
        {
            var config = new BindingConfig();

            var container = new Container(config);
            var e = Assert.Throws<SingularityAggregateException>(() =>
            {
                TestService12WithMixedConcreteDependency value = container.GetInstance<TestService12WithMixedConcreteDependency>();
            });

            Assert.Single(e.InnerExceptions);
            Assert.IsType<DependencyNotFoundException>(e.InnerExceptions.First());
        }

        [Fact]
        public void GetInstance_MissingDecoratorDependency_Throws()
        {
            try
            {
                var config = new BindingConfig();
                config.Register<ITestService10, TestService10>();
                config.Decorate<ITestService10, TestService10_Decorator1>();
                var container = new Container(config);
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
            var container = new Container(new BindingConfig());
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
                var config = new BindingConfig();
                config.Register<ITestService11, TestService11>();
                var container = new Container(config);
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
            var config = new BindingConfig();
            config.Register(typeof(ISerializer<>), typeof(DefaultSerializer<>));
            var nestedConfig = new BindingConfig();
            nestedConfig.Register<ISerializer<int>, IntSerializer>();
            var container = new Container(config);

            //ACT
            //ASSERT
            Assert.Throws<RegistrationAlreadyExistsException>(() =>
            {
                Container nestedContainer = container.GetNestedContainer(nestedConfig);
            });
        }

        [Fact]
        public void DirectlyRegisteredEnumerable_Throws()
        {
            //ARRANGE
            var config = new BindingConfig();
            Assert.Throws<EnumerableRegistrationException>(() =>
            {
                config.Register<IEnumerable<IPlugin>>().Inject(() => new IPlugin[] { new Plugin1(), new Plugin2(), new Plugin3() });
            });
        }
    }
}
