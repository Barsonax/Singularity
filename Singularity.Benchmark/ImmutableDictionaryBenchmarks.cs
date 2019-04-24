using System;
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

        private ImmutableHashTable<ReferenceInt, int> _hashTable;
        private ReferenceInt[] _mapping;

        [GlobalSetup]
        public void Setup()
        {
            _mapping = new ReferenceInt[N];
            for (int i = 0; i < _mapping.Length; i++)
            {
                _mapping[i] = new ReferenceInt(i);
            }
            _hashTable = ImmutableHashTable<ReferenceInt, int>.Empty;
            for (var i = 0; i < N; i++)
            {
                _hashTable = _hashTable.Add(_mapping[i], i);
            }
        }

        //[Benchmark]
        //public void ImmutableHashTable_Get()
        //{
        //    for (var i = 0; i < N; i++)
        //    {
        //        _hashTable.Get(_mapping[i]);
        //    }
        //}

        [Benchmark]
        public void ImmutableHashTable_Add()
        {
            ImmutableHashTable<ReferenceInt, int> hashTable = ImmutableHashTable<ReferenceInt, int>.Empty;
            for (var i = 0; i < N; i++)
            {
                hashTable = hashTable.Add(_mapping[i], i);
            }
        }
    }
}
