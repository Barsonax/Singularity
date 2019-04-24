using BenchmarkDotNet.Attributes;
using Singularity.Collections;
using Singularity.TestClasses.TestClasses;

namespace Singularity.Benchmark
{
    [Config(typeof(CustomJob))]
    [MemoryDiagnoser]
    public class ImmutableDictionaryBenchmarks
    {
        [Params(1, 3, 10, 100, 1000)]
        public int N { get; set; }

        private ImmutableAvlDictionary<ReferenceInt, int> avlDictionary;

        [GlobalSetup]
        public void Setup()
        {
            avlDictionary = ImmutableAvlDictionary<ReferenceInt, int>.Empty;
            for (var i = 0; i < N; i++)
            {
                avlDictionary = avlDictionary.Add(i, i);
            }
        }

        [Benchmark]
        public void LookupDic()
        {
            for (var i = 0; i < N; i++)
            {
                avlDictionary.Get(i);
            }
        }

        [Benchmark]
        public void ImmutableDictionary_Add()
        {
            ImmutableAvlDictionary<ReferenceInt, int> avlDictionary = ImmutableAvlDictionary<ReferenceInt, int>.Empty;
            for (var i = 0; i < N; i++)
            {
                avlDictionary = avlDictionary.Add(i, i);
            }
        }
    }
}
