using System.Linq;
using System.Runtime.CompilerServices;
using Singularity.Benchmark.TestClasses;

namespace Singularity.TestClasses.Benchmark
{
    public class ContainerBenchmark
    {
        private Container _container;

        public void Setup()
        {
            _container = NewContainer();
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
