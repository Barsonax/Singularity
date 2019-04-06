using System;
using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;
using Singularity.TestClasses.Benchmark;
using Singularity.TestClasses.TestClasses;

namespace Singularity.Benchmark
{
    [Config(typeof(CustomJob))]
    [MemoryDiagnoser]
    public class SingularityContainerBenchmark
    {
        private ContainerBenchmark _containerBenchmark;

        [GlobalSetup]
        public void Setup()
        {
            _containerBenchmark = new ContainerBenchmark();
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

        //[Benchmark]
        //public ISimpleAdapter[] MultiEnumerate()
        //{
        //    foreach (ISimpleAdapter obj in _containerBenchmark.Multi())
        //    {
        //    }

        //    return null;
        //}

        //[Benchmark]
        //public void MultiNoEnumeration()
        //{
        //    var foo = _containerBenchmark.Multi();
        //}

        //[Benchmark]
        //public IDisposable Disposable()
        //{
        //    return _containerBenchmark.Disposable();
        //}

        //[Benchmark]
        //public void Register()
        //{
        //    _containerBenchmark.Register();
        //}

        //[Benchmark]
        //public void RegisterAndEnumerate()
        //{
        //    _containerBenchmark.RegisterAndEnumerate();
        //}

        //[Benchmark]
        //public Container NewContainer()
        //{
        //    return _containerBenchmark.NewContainer();
        //}

        //[Benchmark]
        //public Container NewContainerFromCachedConfig()
        //{
        //    return _containerBenchmark.NewContainerFromCachedConfig();
        //}

        //[Benchmark]
        //public IComplex1 NewContainerAndResolve()
        //{
        //    return _containerBenchmark.NewContainerAndResolve();
        //}

        //[Benchmark]
        //public Container NewNestedContainer()
        //{
        //    return _containerBenchmark.NewNestedContainer();
        //}
    }
}