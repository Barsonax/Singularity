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
