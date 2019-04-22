using BenchmarkDotNet.Attributes;
using Singularity.TestClasses.Benchmark;
using Singularity.TestClasses.TestClasses;

namespace Singularity.Benchmark
{
    /// <summary>
    /// Inspired by the simple benchmarks in https://github.com/danielpalme/IocPerformance
    /// </summary>
    [Config(typeof(CustomJob))]
    [MemoryDiagnoser]
    public class SimpleContainerBenchmark
    {
        /// <summary>
        /// Contains the actual code that is benchmarked.
        /// This is because these are also unit tested.
        /// </summary>
        private SimpleSingularityContainerBenchmark _benchmark;

        [GlobalSetup]
        public void Setup()
        {
            _benchmark = new SimpleSingularityContainerBenchmark();
        }

        [Benchmark]
        public ISingleton1 Singleton()
        {
            return _benchmark.Singleton();
        }

        [Benchmark]
        public ITransient1 Transient()
        {
            return _benchmark.Transient();
        }

        [Benchmark]
        public ICombined1 Combined()
        {
            return _benchmark.Combined();
        }

        [Benchmark]
        public IComplex1 Complex()
        {
            return _benchmark.Complex();
        }
    }
}