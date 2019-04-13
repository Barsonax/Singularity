using BenchmarkDotNet.Running;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Singularity.Collections;
using Singularity.TestClasses.Benchmark;
using Singularity.TestClasses.TestClasses;

namespace Singularity.Benchmark
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<FooBenchmark>();
            Console.ReadKey();
        }
    }

    [Config(typeof(CustomJob))]
    [MemoryDiagnoser]
    [DisassemblyDiagnoser(printAsm: true, printSource: true)]
    public class FooBenchmark
    {
        private readonly ThreadSafeDictionary<Type, Func<Scoped, object>> _getInstanceCache = new ThreadSafeDictionary<Type, Func<Scoped, object>>();
        private readonly Scoped _scope = new Scoped();
        private Func<Scoped, Singleton1> compileddel;
        private Container _container;
        private int _hashCode;

        [GlobalSetup]
        public void Setup()
        {
            Singleton1 instance = new Singleton1();
            var expression = Expression.Constant(instance);
            compileddel = (Func<Scoped, Singleton1>)Expression.Lambda(expression, Expression.Parameter(typeof(Scoped))).CompileFast();
            _getInstanceCache.Add(typeof(ISingleton1), compileddel);

            var config = new BindingConfig();
            config.Register<ISingleton1, Singleton1>().With(Lifetime.PerContainer);
            _container = new Container(config);

            _hashCode = RuntimeHelpers.GetHashCode(typeof(ISingleton1));
        }

        //[Benchmark]
        //public ISingleton1 CompiledDelegate()
        //{
        //    return compileddel(_scope);
        //}

        //[Benchmark]
        //public ISingleton1 CompiledDelegateInvoke()
        //{
        //    return compileddel.Invoke(_scope);
        //}

        //[Benchmark]
        //public ISingleton1 Container()
        //{
        //    return (ISingleton1)Container.GetInstance(typeof(ISingleton1));
        //}

        //[Benchmark]
        //public object ContainerNoCast()
        //{
        //    return Container.GetInstance(typeof(ISingleton1));
        //}

        //[Benchmark]
        //public object ContainerNoCastPreCalculatedHash()
        //{
        //    Func<Scoped, object> func = _getInstanceCache.Search(typeof(ISingleton1), _hashCode);
        //    return func(_scope);
        //}

        [Benchmark]
        public object ContainerNoCast()
        {
            Func<Scoped, object> func = _getInstanceCache.Search(typeof(ISingleton1));
            return func(_scope);
        }
    }
}
