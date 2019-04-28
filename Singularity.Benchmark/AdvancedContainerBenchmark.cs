using BenchmarkDotNet.Attributes;
using Singularity.TestClasses.Benchmark;

namespace Singularity.Benchmark
{
    [Config(typeof(CustomJob))]
    [MemoryDiagnoser]
    public class AdvancedContainerBenchmark
    {
        private AdvancedSingularityContainerBenchmark _benchmark;

        [GlobalSetup]
        public void Setup()
        {
            _benchmark = new AdvancedSingularityContainerBenchmark();
        }

        //[Benchmark]
        //public void AspNetCore()
        //{
        //    _benchmark.AspNetCore();
        //}

        //[Benchmark]
        //public ISimpleAdapter[] MultiEnumerate()
        //{
        //    foreach (ISimpleAdapter obj in _benchmark.Multi())
        //    {
        //    }

        //    return null;
        //}

        //[Benchmark]
        //public void MultiNoEnumeration()
        //{
        //    var foo = _benchmark.Multi();
        //}

        //[Benchmark]
        //public IDisposable Disposable()
        //{
        //    return _benchmark.Disposable();
        //}

        //[Benchmark]
        //public void Register()
        //{
        //    _benchmark.Register();
        //}

        //[Benchmark]
        //public Container NewContainer()
        //{
        //    return _benchmark.NewContainer();
        //}

        //[Benchmark]
        //public Container NewContainerFromCachedConfig()
        //{
        //    return _benchmark.NewContainerFromCachedConfig();
        //}

        //[Benchmark]
        //public IComplex1 NewContainerAndResolve()
        //{
        //    return _benchmark.NewContainerAndResolve();
        //}

        //[Benchmark]
        //public Container NewNestedContainer()
        //{
        //    return _benchmark.NewNestedContainer();
        //}
    }
}