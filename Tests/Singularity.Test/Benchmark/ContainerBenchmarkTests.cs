using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Singularity.Benchmark.TestClasses;
using Singularity.Test.TestClasses;
using Singularity.TestClasses.Benchmark;
using Xunit;

namespace Singularity.Test.Benchmark
{
    public class ContainerBenchmarkTests
    {
        private readonly ContainerBenchmark _containerBenchmark = new ContainerBenchmark();

        public ContainerBenchmarkTests()
        {
            _containerBenchmark.Setup();
        }

        [Fact]
        public void Singleton()
        {
            ISingleton1 value = _containerBenchmark.Singleton();

            Assert.IsType<Singleton1>(value);
        }

        [Fact]
        public void Transient()
        {
            ITransient1 value = _containerBenchmark.Transient();

            Assert.IsType<Transient1>(value);
        }

        [Fact]
        public void Combined()
        {
            ICombined1 value = _containerBenchmark.Combined();

            Assert.IsType<Combined1>(value);
        }

        [Fact]
        public void Disposable()
        {
            IDisposable value = _containerBenchmark.Disposable();

            Assert.IsType<Disposable>(value);
        }

        [Fact]
        public void CombinedManual()
        {
            Expression singleton1NewExpression = AutoResolveConstructorExpressionCache<Singleton1>.Expression;
            Delegate action1 = Expression.Lambda(singleton1NewExpression).Compile();
            var value = action1.DynamicInvoke();
            singleton1NewExpression = Expression.Constant(value);

            var transient1NewExpression = AutoResolveConstructorExpressionCache<Transient1>.Expression;

            var expression = Expression.New(typeof(Combined1).AutoResolveConstructor(), singleton1NewExpression, transient1NewExpression);
            Delegate action = Expression.Lambda(expression).Compile();
            var func = (Func<object>)action;
            var instance = func.Invoke();
        }

        [Fact]
        public void Complex()
        {
            IComplex1 value = _containerBenchmark.Complex();

            Assert.IsType<Complex1>(value);
        }

        [Fact]
        public void Register()
        {
            _containerBenchmark.Register();
        }

        [Fact]
        public Container NewContainer()
        {
            return _containerBenchmark.NewContainer();
        }

        [Fact]
        public void NewContainerAndResolve()
        {
            IComplex1 value = _containerBenchmark.NewContainerAndResolve();

            Assert.IsType<Complex1>(value);
        }

        [Fact]
        public Container NewNestedContainer()
        {
            return _containerBenchmark.NewNestedContainer();
        }
    }
}
