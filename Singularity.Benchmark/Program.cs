using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Horology;
using BenchmarkDotNet.Jobs;
using Singularity.Benchmark.TestClasses;
using System.Linq;
using Singularity.Bindings;

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

    public class CustomJob : ManualConfig
    {
        public CustomJob()
        {
            Add(
                Job.RyuJitX64
                    .With(Runtime.Core)
                    .WithIterationTime(TimeInterval.FromMilliseconds(100)));

            Add(
                Job.LegacyJitX64
                    .With(Runtime.Clr)
                    .WithIterationTime(TimeInterval.FromMilliseconds(100)));
        }
    }
    [Config(typeof(CustomJob))]
    [MemoryDiagnoser]
    public class InjectorBenchmark
    {
        private Container _container;

        [GlobalSetup]
        public void Setup()
        {
            _container = NewContainer();
        }

        [Benchmark]
        public ISingleton1 Singleton()
        {
            return (ISingleton1)_container.GetInstance(typeof(ISingleton1));
        }

        [Benchmark]
        public ITransient1 Transient()
        {
            return (ITransient1)_container.GetInstance(typeof(ITransient1));
        }

        [Benchmark]
        public ICombined1 Combined()
        {
            return (ICombined1)_container.GetInstance(typeof(ICombined1));
        }

        [Benchmark]
        public IComplex1 Complex()
        {
            return (IComplex1)_container.GetInstance(typeof(IComplex1));
        }

        [Benchmark]
        public void Register()
        {
            var config = new BindingConfig();

            RegisterDummies(config);
            RegisterStandard(config);
            RegisterComplex(config);
        }

        [Benchmark]
        public Container NewContainer()
        {
            var config = new BindingConfig();

            RegisterDummies(config);
            RegisterStandard(config);
            RegisterComplex(config);

            return new Container(config);
        }

        [Benchmark]
        public IComplex1 NewContainerAndResolve()
        {
            var config = new BindingConfig();

            RegisterDummies(config);
            RegisterStandard(config);
            RegisterComplex(config);

            var container = new Container(config);
            return (IComplex1)container.GetInstance(typeof(IComplex1));
        }

        [Benchmark]
        public Container NewNestedContainer()
        {
            return _container.GetNestedContainer(Enumerable.Empty<IModule>());
        }

        private static void RegisterDummies(BindingConfig config)
        {
            config.Register<IDummyOne, DummyOne>();
            config.Register<IDummyTwo, DummyTwo>();
            config.Register<IDummyThree, DummyThree>();
            config.Register<IDummyFour, DummyFour>();
            config.Register<IDummyFive, DummyFive>();
            config.Register<IDummySix, DummySix>();
            config.Register<IDummySeven, DummySeven>();
            config.Register<IDummyEight, DummyEight>();
            config.Register<IDummyNine, DummyNine>();
            config.Register<IDummyTen, DummyTen>();
        }

        private static void RegisterStandard(BindingConfig config)
        {
            config.Register<ISingleton1, Singleton1>().With(Lifetimes.Singleton);
            config.Register<ISingleton2, Singleton2>().With(Lifetimes.Singleton);
            config.Register<ISingleton3, Singleton3>().With(Lifetimes.Singleton);
            config.Register<ITransient1, Transient1>();
            config.Register<ITransient2, Transient2>();
            config.Register<ITransient3, Transient3>();
            config.Register<ICombined1, Combined1>();
            config.Register<ICombined2, Combined2>();
            config.Register<ICombined3, Combined3>();
            config.Register<ICalculator1, Calculator1>();
            config.Register<ICalculator2, Calculator2>();
            config.Register<ICalculator3, Calculator3>();
        }


        private static void RegisterComplex(BindingConfig config)
        {
            config.Register<ISubObjectOne, SubObjectOne>();
            config.Register<ISubObjectTwo, SubObjectTwo>();
            config.Register<ISubObjectThree, SubObjectThree>();
            config.Register<IFirstService, FirstService>().With(Lifetimes.Singleton);
            config.Register<ISecondService, SecondService>().With(Lifetimes.Singleton);
            config.Register<IThirdService, ThirdService>().With(Lifetimes.Singleton);
            config.Register<IComplex1, Complex1>();
        }
    }
}
