using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Singularity.TestClasses.Benchmark;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Benchmark
{
    public class ContainerBenchmarkTests
    {
        private readonly ContainerBenchmark _containerBenchmark = new ContainerBenchmark();

        [Fact]
        public void Singleton()
        {
            var foo = typeof(ISingleton1);
            var type1 = Type.GetType(foo.AssemblyQualifiedName);
            var type2 = Type.GetType(foo.AssemblyQualifiedName);
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
            object value = action1.DynamicInvoke();
            singleton1NewExpression = Expression.Constant(value);

            Expression transient1NewExpression = AutoResolveConstructorExpressionCache<Transient1>.Expression;

            NewExpression expression = Expression.New(typeof(Combined1).AutoResolveConstructor(), singleton1NewExpression, transient1NewExpression);
            Delegate action = Expression.Lambda(expression).Compile();
            var func = (Func<object>)action;
            object instance = func.Invoke();
        }

        [Fact]
        public void AspNetCore()
        {
            _containerBenchmark.AspNetCore();
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

