using BenchmarkDotNet.Attributes;

namespace Singularity.Benchmark
{
    [Config(typeof(CustomJob))]
    [MemoryDiagnoser]
    public class DisposeListBenchmarks
    {
        [Params(1, 3, 10, 100, 1000)]
        public int N { get; set; }

        private DisposeList<object> _disposeList;

        [GlobalSetup]
        public void Setup()
        {
            _disposeList = new DisposeList<object>(obj => Nothing());

            for (var i = 0; i < N; i++)
            {
                _disposeList.Add(new object());
            }
        }

        private void Nothing() { }

        [Benchmark]
        public void DisposeList_Invoke()
        {
            _disposeList.Invoke();
        }

        [Benchmark]
        public void DisposeList_Add()
        {
            var disposeList = new DisposeList<object>(obj => Nothing());

            for (var i = 0; i < N; i++)
            {
                disposeList.Add(new object());
            }
        }
    }
}
