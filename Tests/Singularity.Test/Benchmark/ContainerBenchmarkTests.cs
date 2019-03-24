using Singularity.Benchmark.TestClasses;
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
            var value = _containerBenchmark.Singleton();

            Assert.IsType<Singleton1>(value);
        }

        [Fact]
        public void Transient()
        {
            var value = _containerBenchmark.Transient();

            Assert.IsType<Transient1>(value);
        }

        [Fact]
        public void Combined()
        {
            var value = _containerBenchmark.Combined();

            Assert.IsType<Combined1>(value);
        }

        [Fact]
        public void Complex()
        {
            var value = _containerBenchmark.Complex();

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
            var value = _containerBenchmark.NewContainerAndResolve();

            Assert.IsType<Complex1>(value);
        }

        [Fact]
        public Container NewNestedContainer()
        {
            return _containerBenchmark.NewNestedContainer();
        }
    }
}
