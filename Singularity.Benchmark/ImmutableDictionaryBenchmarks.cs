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

        private ImmutableDictionary<ReferenceInt, int> _dictionary;

        [GlobalSetup]
        public void Setup()
        {
            _dictionary = ImmutableDictionary<ReferenceInt, int>.Empty;
            for (var i = 0; i < N; i++)
            {
                _dictionary = _dictionary.Add(i, i);
            }
        }

        [Benchmark]
        public void LookupDic()
        {
            for (var i = 0; i < N; i++)
            {
                _dictionary.Get(i);
            }
        }

        [Benchmark]
        public void ImmutableDictionary_Add()
        {
            ImmutableDictionary<ReferenceInt, int> dictionary = ImmutableDictionary<ReferenceInt, int>.Empty;
            for (var i = 0; i < N; i++)
            {
                dictionary = dictionary.Add(i, i);
            }
        }
    }
}
