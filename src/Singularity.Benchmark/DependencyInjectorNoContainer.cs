using BenchmarkDotNet.Attributes;
using Singularity.TestClasses.TestClasses;

namespace Singularity.Benchmark
{
    [Config(typeof(CustomJob))]
    [MemoryDiagnoser]
    public class DependencyInjectorNoContainer
    {
        private Singleton1 _singleton1 = new Singleton1();
        [Benchmark]
        public ISingleton1 SingletonDirect()
        {
            return _singleton1;
        }

        [Benchmark]
        public ITransient1 TransientDirect()
        {
            return new Transient1();
        }

        [Benchmark]
        public ICombined1 CombinedDirect()
        {
            return new Combined1(_singleton1, new Transient1());
        }

        private FirstService _firstService = new FirstService();
        private SecondService _secondService = new SecondService();
        private ThirdService _thirdService = new ThirdService();
        [Benchmark]
        public IComplex1 ComplexDirect()
        {
            return new Complex1(_firstService, _secondService, _thirdService, new SubObjectOne(_firstService), new SubObjectTwo(_secondService), new SubObjectThree(_thirdService));
        }
    }
}
