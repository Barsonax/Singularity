using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Singularity.Collections;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Collections
{
    public class ImmutableHashTableTests
    {
        [Theory]
        [ClassData(typeof(DictionaryTypeTheoryData))]
        public void SearchType(IEnumerable<(Type key, ReferenceInt value)> testValues)
        {
            ImmutableHashTable<Type, ReferenceInt> avlDictionary = ImmutableHashTable<Type, ReferenceInt>.Empty;

            foreach ((Type key, ReferenceInt value) in testValues)
            {
                avlDictionary = avlDictionary.Add(key, value);
            }

            foreach ((Type key, ReferenceInt value) in testValues)
            {
                Assert.Equal(value, avlDictionary.Get(key));
            }
        }

        [Theory]
        [ClassData(typeof(DictionaryTestsTheoryData))]
        public void SearchReferenceInt(IEnumerable<(ReferenceInt key, ReferenceInt value)> testValues)
        {
            ImmutableHashTable<ReferenceInt, ReferenceInt> avlDictionary = ImmutableHashTable<ReferenceInt, ReferenceInt>.Empty;

            foreach ((ReferenceInt key, ReferenceInt value) in testValues)
            {
                avlDictionary = avlDictionary.Add(key, value);
            }

            foreach ((ReferenceInt key, ReferenceInt value) in testValues)
            {
                Assert.Equal(value, avlDictionary.Get(key));
            }
        }

        [Theory]
        [ClassData(typeof(DictionaryTestsTheoryData))]
        public void EnumerateGeneric(IEnumerable<(ReferenceInt key, ReferenceInt value)> testValues)
        {
            ImmutableHashTable<ReferenceInt, ReferenceInt> avlDictionary = ImmutableHashTable<ReferenceInt, ReferenceInt>.Empty;

            foreach ((ReferenceInt key, ReferenceInt value) in testValues)
            {
                avlDictionary = avlDictionary.Add(key, value);
            }

            ReferenceInt[] values = avlDictionary.ToArray();

            Assert.NotStrictEqual(testValues.Select(x => x.value), values);
        }

        [Theory]
        [ClassData(typeof(DictionaryTestsTheoryData))]
        public void Enumerate(IEnumerable<(ReferenceInt key, ReferenceInt value)> testValues)
        {
            ImmutableHashTable<ReferenceInt, ReferenceInt> avlDictionary = ImmutableHashTable<ReferenceInt, ReferenceInt>.Empty;

            foreach ((ReferenceInt key, ReferenceInt value) in testValues)
            {
                avlDictionary = avlDictionary.Add(key, value);
            }

            var enumerable = (IEnumerable)avlDictionary;
            ReferenceInt[] values = enumerable.OfType<ReferenceInt>().ToArray();

            Assert.NotStrictEqual(testValues.Select(x => x.value), values);
        }
    }
}
