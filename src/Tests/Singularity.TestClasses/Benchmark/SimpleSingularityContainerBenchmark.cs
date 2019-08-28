using Singularity.TestClasses.TestClasses;

namespace Singularity.TestClasses.Benchmark
{
    public class SimpleSingularityContainerBenchmark
    {
        private readonly Container _container;

        public SimpleSingularityContainerBenchmark()
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

        public Container NewContainer()
        {
            return new Container(Register);
        }

        private static void Register(ContainerBuilder config)
        {
            config.Register<ISingleton1, Singleton1>(c => c.With(Lifetimes.PerContainer));
            config.Register<ISingleton2, Singleton2>(c => c.With(Lifetimes.PerContainer));
            config.Register<ISingleton3, Singleton3>(c => c.With(Lifetimes.PerContainer));

            config.Register<ITransient1, Transient1>();
            config.Register<ITransient2, Transient2>();
            config.Register<ITransient3, Transient3>();

            config.Register<ICombined1, Combined1>();
            config.Register<ICombined2, Combined2>();
            config.Register<ICombined3, Combined3>();

            config.Register<ISubObjectOne, SubObjectOne>();
            config.Register<ISubObjectTwo, SubObjectTwo>();
            config.Register<ISubObjectThree, SubObjectThree>();
            config.Register<IFirstService, FirstService>(c => c.With(Lifetimes.PerContainer));
            config.Register<ISecondService, SecondService>(c => c.With(Lifetimes.PerContainer));
            config.Register<IThirdService, ThirdService>(c => c.With(Lifetimes.PerContainer));
            config.Register<IComplex1, Complex1>();
        }
    }
}