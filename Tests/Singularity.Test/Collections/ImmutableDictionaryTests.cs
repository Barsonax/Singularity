using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Singularity.Collections;
using Singularity.Test.TestClasses;
using Xunit;

namespace Singularity.Test.Collections
{
    public class ImmutableDictionaryTests
    {       
        [Theory]
        [ClassData(typeof(ImmutableDictionaryTestsTheoryData))]
        public void Search(IEnumerable<(Type type, int value)> testValues)
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
    }

    public class ImmutableDictionaryTestsTheoryData : TheoryData<IEnumerable<(Type type, int value)>>
    {
        public ImmutableDictionaryTestsTheoryData()
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
