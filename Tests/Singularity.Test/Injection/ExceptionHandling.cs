using System;
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

                Assert.Equal(typeof(int), dependencyNotFoundException.Type);
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

                Assert.Equal(typeof(ITestService10), dependencyNotFoundException.Type);
            }
        }
    }
}
