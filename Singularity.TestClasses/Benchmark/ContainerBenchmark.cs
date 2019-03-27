using System;
using System.Linq;
using Singularity.Benchmark.TestClasses;
using Singularity.Test.TestClasses;

namespace Singularity.TestClasses.Benchmark
{
    public class ContainerBenchmark
    {
        private Container _container;
        private BindingConfig _cachedBindingConfig;

        public void Setup()
        {
            _container = NewContainer();
            _cachedBindingConfig = new BindingConfig();
            Register(_cachedBindingConfig);
        }

        public ISingleton1 Singleton()
        {
            return (ISingleton1)_container.GetInstance(typeof(ISingleton1));
        }

        public ITransient1 Transient()
        {
            return (ITransient1)_container.GetInstance(typeof(ITransient1));
        }

        public ICombined1 Combined()
        {
            return (ICombined1)_container.GetInstance(typeof(ICombined1));
        }

        public IComplex1 Complex()
        {
            return (IComplex1)_container.GetInstance(typeof(IComplex1));
        }

        public IDisposable Disposable()
        {
            return (IDisposable)_container.GetInstance(typeof(IDisposable));
        }

        public void Register()
        {
            var config = new BindingConfig();

            Register(config);
        }

        public void RegisterAndEnumerate()
        {
            var config = new BindingConfig();

            Register(config);

            config.GetDependencies();
        }

        public Container NewContainer()
        {
            var config = new BindingConfig();

            Register(config);

            return new Container(config);
        }

        public Container NewContainerFromCachedConfig()
        {
            return new Container(_cachedBindingConfig);
        }

        public IComplex1 NewContainerAndResolve()
        {
            var config = new BindingConfig();

            Register(config);

            var container = new Container(config);
            return (IComplex1)container.GetInstance(typeof(IComplex1));
        }

        public Container NewNestedContainer()
        {
            return _container.GetNestedContainer(Enumerable.Empty<IModule>());
        }

        private static void Register(BindingConfig config)
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

            config.Register<ISingleton1, Singleton1>().With(CreationMode.Singleton);
            config.Register<ISingleton2, Singleton2>().With(CreationMode.Singleton);
            config.Register<ISingleton3, Singleton3>().With(CreationMode.Singleton);
            config.Register<ITransient1, Transient1>();
            config.Register<ITransient2, Transient2>();
            config.Register<ITransient3, Transient3>();
            config.Register<ICombined1, Combined1>();
            config.Register<ICombined2, Combined2>();
            config.Register<ICombined3, Combined3>();
            config.Register<ICalculator1, Calculator1>();
            config.Register<ICalculator2, Calculator2>();
            config.Register<ICalculator3, Calculator3>();

            config.Register<ISubObjectOne, SubObjectOne>();
            config.Register<ISubObjectTwo, SubObjectTwo>();
            config.Register<ISubObjectThree, SubObjectThree>();
            config.Register<IFirstService, FirstService>().With(CreationMode.Singleton);
            config.Register<ISecondService, SecondService>().With(CreationMode.Singleton);
            config.Register<IThirdService, ThirdService>().With(CreationMode.Singleton);
            config.Register<IComplex1, Complex1>();

            config.Register<IDisposable, Disposable>().OnDeath(x => x.Dispose());
        }
    }
}
