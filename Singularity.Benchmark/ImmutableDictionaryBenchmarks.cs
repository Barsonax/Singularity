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

        private Hashtable<ReferenceInt, int> _table;
        private ImmutableDictionary<ReferenceInt, int> _dictionary;

        [GlobalSetup]
        public void Setup()
        {
            _table = Hashtable<ReferenceInt, int>.Empty;
            for (var i = 0; i < N; i++)
            {
                _table = _table.Add(i, i);
            }

            _dictionary = ImmutableDictionary<ReferenceInt, int>.Empty;
            for (var i = 0; i < N; i++)
            {
                _dictionary = _dictionary.Add(i, i);
            }
        }

        //[Benchmark]
        //public void LookupDic()
        //{
        //    for (var i = 0; i < N; i++)
        //    {
        //        _dictionary.Get(i);
        //    }
        //}

        //[Benchmark]
        //public void LookupTable()
        //{
        //    for (var i = 0; i < N; i++)
        //    {
        //        _table.Get(i);
        //    }
        //}

        //[Benchmark]
        //public void AddHashtable()
        //{
        //    Hashtable<ReferenceInt, int> table = Hashtable<ReferenceInt, int>.Empty;
        //    for (var i = 0; i < N; i++)
        //    {
        //        table = table.Add(i, i);
        //    }
        //}

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
