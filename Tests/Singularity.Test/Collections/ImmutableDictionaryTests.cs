using System;
using System.Collections.Generic;
using Singularity.Collections;
using Singularity.TestClasses.Extensions;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Collections
{
    public class HashTableTests
    {
        [Fact]
        public void Add()
        {
            ImmutableDictionary<Type, int> hashtable = ImmutableDictionary<Type, int>.Empty;
            hashtable = hashtable.Add(typeof(ITestService10), 0);
            hashtable = hashtable.Add(typeof(ITestService11), 1);
            hashtable = hashtable.Add(typeof(ITestService12), 2);
        }

        [Fact]
        public void Get()
        {
            ImmutableDictionary<Type, int> hashtable = ImmutableDictionary<Type, int>.Empty;
            hashtable = hashtable.Add(typeof(ITestService10), 0);
            hashtable = hashtable.Add(typeof(ITestService11), 1);
            hashtable = hashtable.Add(typeof(ITestService12), 2);

            int test10 = hashtable.Get(typeof(ITestService10));
            int test11 = hashtable.Get(typeof(ITestService11));
            int test12 = hashtable.Get(typeof(ITestService12));

            Assert.Equal(0, test10);
            Assert.Equal(1, test11);
            Assert.Equal(2, test12);

        }

        [Fact]
        public void GetNoValues()
        {
            ImmutableDictionary<Type, int> hashtable = ImmutableDictionary<Type, int>.Empty;

            int test10 = hashtable.Get(typeof(ITestService10));

            Assert.Equal(0, test10);
        }
    }

    public class ImmutableDictionaryTests
    {
        [Theory]
        [ClassData(typeof(ImmutableDictionaryTestsReferenceTypeTheoryData))]
        public void SearchReferenceType(IEnumerable<(Type type, ReferenceInt value)> testValues)
        {
            ImmutableDictionary<Type, ReferenceInt> hashtable = ImmutableDictionary<Type, ReferenceInt>.Empty;

            foreach ((Type type, ReferenceInt value) in testValues)
            {
                hashtable = hashtable.Add(type, value);
            }

            foreach ((Type type, ReferenceInt value) in testValues)
            {
                Assert.Equal(value, hashtable.Get(type));
            }
        }

        [Theory]
        [ClassData(typeof(ImmutableDictionaryTestsValueTypeTheoryData))]
        public void SearchValueType(IEnumerable<(ReferenceInt key, ReferenceInt value)> testValues)
        {
            ImmutableDictionary<ReferenceInt, ReferenceInt> hashtable = ImmutableDictionary<ReferenceInt, ReferenceInt>.Empty;

            foreach ((ReferenceInt key, ReferenceInt value) in testValues)
            {
                hashtable = hashtable.Add(key, value);
            }

            foreach ((ReferenceInt key, ReferenceInt value) in testValues)
            {
                Assert.Equal(value, hashtable.Get(key));
            }
        }
    }

    public class ImmutableDictionaryTestsValueTypeTheoryData : TheoryData<IEnumerable<(ReferenceInt key, ReferenceInt value)>>
    {
        public ImmutableDictionaryTestsValueTypeTheoryData()
        {
            var typeMappings = new List<(ReferenceInt key, ReferenceInt value)>();
            typeMappings.Add((0, 0));
            typeMappings.Add((1, 1));
            typeMappings.Add((2, 2));
            typeMappings.Add((3, 3));
            typeMappings.Add((4, 4));

            IEnumerable<IEnumerable<(ReferenceInt key, ReferenceInt value)>> permutations = typeMappings.GetPermutations();

            foreach (IEnumerable<(ReferenceInt key, ReferenceInt value)> permutation in permutations)
            {
                Add(permutation);
            }
        }
    }

    public class ImmutableDictionaryTestsReferenceTypeTheoryData : TheoryData<IEnumerable<(Type type, ReferenceInt value)>>
    {
        public ImmutableDictionaryTestsReferenceTypeTheoryData()
        {
            var typeMappings = new List<(Type type, ReferenceInt value)>();
            typeMappings.Add((typeof(ITestService10), 0));
            typeMappings.Add((typeof(ITestService11), 1));
            typeMappings.Add((typeof(ITestService12), 2));

            typeMappings.Add((typeof(ITestService20), 4));

            IEnumerable<IEnumerable<(Type type, ReferenceInt value)>> permutations = typeMappings.GetPermutations();

            foreach (IEnumerable<(Type type, ReferenceInt value)> permutation in permutations)
            {
                Add(permutation);
            }
        }
    }
}
