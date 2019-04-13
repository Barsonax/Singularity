using System;
using System.Collections.Generic;
using Singularity.Collections;
using Singularity.TestClasses.Extensions;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Collections
{
    public class ImmutableDictionaryTests
    {
        [Theory]
        [ClassData(typeof(ImmutableDictionaryTestsReferenceTypeTheoryData))]
        public void SearchReferenceType(IEnumerable<(Type type, int value)> testValues)
        {
            ImmutableDictionary<Type, int> dic = ImmutableDictionary<Type, int>.Empty;

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
        public void SearchValueType(IEnumerable<(ReferenceInt key, ReferenceInt value)> testValues)
        {
            ImmutableDictionary<ReferenceInt, ReferenceInt> dic = ImmutableDictionary<ReferenceInt, ReferenceInt>.Empty;

            foreach ((ReferenceInt key, ReferenceInt value) in testValues)
            {
                dic = dic.Add(key, value);
            }

            foreach ((ReferenceInt key, ReferenceInt value) in testValues)
            {
                Assert.Equal(value, dic.Search(key));
            }
        }
    }

    public class ReferenceInt
    {
        public readonly int Value;
        public ReferenceInt(int value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator ReferenceInt(int value)
        {
            return new ReferenceInt(value);
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

            typeMappings.Add((4, 4));

            IEnumerable<IEnumerable<(ReferenceInt key, ReferenceInt value)>> permutations = typeMappings.GetPermutations();

            foreach (IEnumerable<(ReferenceInt key, ReferenceInt value)> permutation in permutations)
            {
                Add(permutation);
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

            IEnumerable<IEnumerable<(Type type, int value)>> permutations = typeMappings.GetPermutations();

            foreach (IEnumerable<(Type type, int value)> permutation in permutations)
            {
                Add(permutation);
            }
        }
    }
}
