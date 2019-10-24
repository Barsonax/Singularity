using System;
using System.Linq.Expressions;
using Singularity.Expressions;
using Singularity.TestClasses.Benchmark;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Benchmark
{
    public class AdvancedSingularityContainerBenchmarkTests
    {
        private readonly AdvancedSingularityContainerBenchmark advancedSingularityContainerBenchmark = new AdvancedSingularityContainerBenchmark();

        [Fact]
        public void Disposable()
        {
            IDisposable value = advancedSingularityContainerBenchmark.Disposable();

            Assert.IsType<Disposable>(value);
        }

        [Fact]
        public void CombinedManual()
        {
            Expression singleton1NewExpression = ConstructorSelectors.Default.AutoResolveConstructorExpression(typeof(Singleton1));
            Delegate action1 = Expression.Lambda(singleton1NewExpression).Compile();
            object value = action1.DynamicInvoke();
            singleton1NewExpression = Expression.Constant(value);

            Expression transient1NewExpression = ConstructorSelectors.Default.AutoResolveConstructorExpression(typeof(Transient1));

            NewExpression expression = Expression.New(ConstructorSelectors.Default.SelectConstructor(typeof(Combined1)), singleton1NewExpression, transient1NewExpression);
            Delegate action = Expression.Lambda(expression).Compile();
            var func = (Func<object>)action;
            object instance = func.Invoke();
        }

        [Fact]
        public void AspNetCore()
        {
            advancedSingularityContainerBenchmark.AspNetCore();
        }

        [Fact]
        public void Register()
        {
            advancedSingularityContainerBenchmark.Register();
        }

        [Fact]
        public Container NewContainer()
        {
            return advancedSingularityContainerBenchmark.NewContainer();
        }

        [Fact]
        public void NewContainerAndResolve()
        {
            IComplex1 value = advancedSingularityContainerBenchmark.NewContainerAndResolve();

            Assert.IsType<Complex1>(value);
        }

        [Fact]
        public Container NewNestedContainer()
        {
            return advancedSingularityContainerBenchmark.NewNestedContainer();
        }
    }
}

