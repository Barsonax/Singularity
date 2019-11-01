using BenchmarkDotNet.Attributes;
using Singularity.Collections;

namespace Singularity.Benchmark
{
    [Config(typeof(CustomJob))]
    [MemoryDiagnoser]
    public class DisposeListBenchmarks
    {
        [Params(1, 3, 10, 100, 1000)]
        public int N { get; set; }

        private ActionList<object> actionList;

        [GlobalSetup]
        public void Setup()
        {
            actionList = new ActionList<object>(obj => Nothing());

            for (var i = 0; i < N; i++)
            {
                actionList.Add(new object());
            }
        }

        private void Nothing() { }

        [Benchmark]
        public void DisposeList_Invoke()
        {
            actionList.Invoke();
        }

        [Benchmark]
        public void DisposeList_Add()
        {
            var disposeList = new ActionList<object>(obj => Nothing());

            for (var i = 0; i < N; i++)
            {
                disposeList.Add(new object());
            }
        }
    }
}
