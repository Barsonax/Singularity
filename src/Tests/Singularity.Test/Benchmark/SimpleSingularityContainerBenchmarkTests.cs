using Singularity.TestClasses.Benchmark;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Benchmark
{
    public class SimpleSingularityContainerBenchmarkTests
    {
        private SimpleSingularityContainerBenchmark _benchmark = new SimpleSingularityContainerBenchmark();

        [Fact]
        public void Singleton()
        {
            ISingleton1 value = _benchmark.Singleton();

            Assert.IsType<Singleton1>(value);
        }

        [Fact]
        public void Transient()
        {
            ITransient1 value = _benchmark.Transient();

            Assert.IsType<Transient1>(value);
        }

        [Fact]
        public void Combined()
        {
            ICombined1 value = _benchmark.Combined();

            Assert.IsType<Combined1>(value);
        }

        [Fact]
        public void Complex()
        {
            IComplex1 value = _benchmark.Complex();

            Assert.IsType<Complex1>(value);
        }
    }
}