using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using Singularity.Benchmark.TestClasses;
using Singularity.TestClasses.Benchmark;

namespace Singularity.Benchmark
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<InjectorBenchmark>();
            Console.ReadKey();
        }
    }

    [Config(typeof(CustomJob))]
    [MemoryDiagnoser]
    public class InjectorBenchmark
    {
        private readonly ContainerBenchmark _containerBenchmark = new ContainerBenchmark();

        [GlobalSetup]
        public void Setup()
        {
            _containerBenchmark.Setup();
        }

        [Benchmark]
        public ISingleton1 Singleton()
        {
            return _containerBenchmark.Singleton();
        }

        [Benchmark]
        public ITransient1 Transient()
        {
            return _containerBenchmark.Transient();
        }

        [Benchmark]
        public ICombined1 Combined()
        {
            return _containerBenchmark.Combined();
        }

        [Benchmark]
        public IComplex1 Complex()
        {
            return _containerBenchmark.Complex();
        }

        [Benchmark]
        public void Register()
        {
            _containerBenchmark.Register();
        }

        [Benchmark]
        public void RegisterAndEnumerate()
        {
            _containerBenchmark.RegisterAndEnumerate();
        }

        [Benchmark]
        public Container NewContainer()
        {
            return _containerBenchmark.NewContainer();
        }

        [Benchmark]
        public IComplex1 NewContainerAndResolve()
        {
            return _containerBenchmark.NewContainerAndResolve();
        }

        [Benchmark]
        public Container NewNestedContainer()
        {
            return _containerBenchmark.NewNestedContainer();
        }
    }
}
