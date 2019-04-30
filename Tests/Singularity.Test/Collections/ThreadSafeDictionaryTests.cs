using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Singularity.Collections;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Collections
{
    public class ThreadSafeDictionaryTests
    {
        [Theory]
        [ClassData(typeof(DictionaryTypeTheoryData))]
        public void SearchType(IEnumerable<(Type key, ReferenceInt value)> testValues)
        {
            var dictionary = new ThreadSafeDictionary<Type, ReferenceInt>();

            foreach ((Type key, ReferenceInt value) in testValues)
            {
                dictionary.Add(key, value);
            }

            foreach ((Type key, ReferenceInt value) in testValues)
            {
                Assert.Equal(value, dictionary.GetOrDefault(key));
            }
        }

        [Theory]
        [ClassData(typeof(DictionaryTestsTheoryData))]
        public void SearchReferenceInt(IEnumerable<(ReferenceInt key, ReferenceInt value)> testValues)
        {
            var dictionary = new ThreadSafeDictionary<ReferenceInt, ReferenceInt>();

            foreach ((ReferenceInt key, ReferenceInt value) in testValues)
            {
                dictionary.Add(key, value);
            }

            foreach ((ReferenceInt key, ReferenceInt value) in testValues)
            {
                if (value.Value == 670 && key.Value == 228)
                {

                }
                Assert.Equal(value, dictionary.GetOrDefault(key));
            }
        }

        [Theory]
        [ClassData(typeof(DictionaryTestsTheoryData))]
        public void EnumerateGeneric(IEnumerable<(ReferenceInt key, ReferenceInt value)> testValues)
        {
            var dictionary = new ThreadSafeDictionary<ReferenceInt, ReferenceInt>();

            foreach ((ReferenceInt key, ReferenceInt value) in testValues)
            {
                dictionary.Add(key, value);
            }

            ReferenceInt[] values = dictionary.ToArray();

            Assert.NotStrictEqual(testValues.Select(x => x.value), values);
        }

        [Theory]
        [ClassData(typeof(DictionaryTestsTheoryData))]
        public void Enumerate(IEnumerable<(ReferenceInt key, ReferenceInt value)> testValues)
        {
            var dictionary = new ThreadSafeDictionary<ReferenceInt, ReferenceInt>();

            foreach ((ReferenceInt key, ReferenceInt value) in testValues)
            {
                dictionary.Add(key, value);
            }

            var enumerable = (IEnumerable) dictionary;
            ReferenceInt[] values = enumerable.OfType<ReferenceInt>().ToArray();

            Assert.NotStrictEqual(testValues.Select(x => x.value), values);
        }
    }
}
