using System;
using System.Collections.Generic;
using System.Linq;
using Singularity.Collections;
using Singularity.Test.TestClasses;
using Xunit;

namespace Singularity.Test.Collections
{
    public class ImmutableDictionaryTests
    {
        [Theory]
        [ClassData(typeof(ImmutableDictionaryTestsReferenceTypeTheoryData))]
        public void SearchReferenceType(IEnumerable<(Type type, int value)> testValues)
        {
            var dic = ImmutableDictionary<Type, int>.Empty;

            foreach ((Type type, int value) in testValues)
            {
                dic = dic.Add(type, value);
            }

            foreach ((Type type, int value) in testValues)
            {
                Assert.Equal(value, dic.Search(type));
            }
        }

        [Theory]
        [ClassData(typeof(ImmutableDictionaryTestsValueTypeTheoryData))]
        public void SearchValueType(IEnumerable<(int key, int value)> testValues)
        {
            var dic = ImmutableDictionary<int, int>.Empty;

            foreach ((int key, int value) in testValues)
            {
                dic = dic.Add(key, value);
            }

            foreach ((int key, int value) in testValues)
            {
                Assert.Equal(value, dic.Search(key));
            }
        }
    }

    public class ImmutableDictionaryTestsValueTypeTheoryData : TheoryData<IEnumerable<(int key, int value)>>
    {
        public ImmutableDictionaryTestsValueTypeTheoryData()
        {
            var typeMappings = new List<(int key, int value)>();
            typeMappings.Add((0, 0));
            typeMappings.Add((1, 1));
            typeMappings.Add((2, 2));

            typeMappings.Add((4, 4));

            var permutations = GetPermutations(typeMappings);

            foreach (IEnumerable<(int key, int value)> permutation in permutations)
            {
                Add(permutation);
            }
        }

        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> items)
        {
            if (items.Count() > 1)
            {
                return items.SelectMany(item => GetPermutations(items.Where(i => !i.Equals(item))),
                    (item, permutation) => new[] { item }.Concat(permutation));
            }
            else
            {
                return new[] { items };
            }
        }
    }

    public class ImmutableDictionaryTestsReferenceTypeTheoryData : TheoryData<IEnumerable<(Type type, int value)>>
    {
        public ImmutableDictionaryTestsReferenceTypeTheoryData()
        {
            var typeMappings = new List<(Type type, int value)>();
            typeMappings.Add((typeof(ITestService10), 0));
            typeMappings.Add((typeof(ITestService11), 1));
            typeMappings.Add((typeof(ITestService12), 2));

            typeMappings.Add((typeof(ITestService20), 4));

            var permutations = GetPermutations(typeMappings);

            foreach (IEnumerable<(Type type, int value)> permutation in permutations)
            {
                Add(permutation);
            }
        }

        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> items)
        {
            if (items.Count() > 1)
            {
                return items.SelectMany(item => GetPermutations(items.Where(i => !i.Equals(item))),
                    (item, permutation) => new[] { item }.Concat(permutation));
            }
            else
            {
                return new[] { items };
            }
        }
    }
}
