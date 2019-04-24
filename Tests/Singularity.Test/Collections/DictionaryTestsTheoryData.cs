using System;
using System.Collections.Generic;
using Singularity.TestClasses.Extensions;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Collections
{
    public class DictionaryTestsTheoryData : TheoryData<IEnumerable<(ReferenceInt key, ReferenceInt value)>>
    {
        public DictionaryTestsTheoryData()
        {
            const int maxValue = 1000;
            const int cases = 50;
            var mapping = new ReferenceInt[maxValue];
            for (var i = 0; i < maxValue; i++)
            {
                mapping[i] = new ReferenceInt(i);
            }
            var sequenceLengthGenerator = new Random(0);
            var generator = new Random(1);

            var sequences = new (ReferenceInt key, ReferenceInt value)[cases][];
            for (int i = 0; i < cases; i++)
            {
                var sequence = new (ReferenceInt key, ReferenceInt value)[sequenceLengthGenerator.Next(0, 110)];

                var usedKeys = new HashSet<int>();
                for (int j = 0; j < sequence.Length; j++)
                {
                    int key;

                    while (true)
                    {
                        key = generator.Next(maxValue);
                        bool isUnique = !usedKeys.Contains(key);
                        usedKeys.Add(key);
                        if (isUnique) break;
                    }

                    sequence[j] = (mapping[key], mapping[generator.Next(maxValue)]);
                }

                sequences[i] = sequence;
            }
            //var typeMappings = new List<(ReferenceInt key, ReferenceInt value)>();
            //typeMappings.Add((484, 0));
            //typeMappings.Add((356, 1));
            //typeMappings.Add((868, 2));
            //typeMappings.Add((228, 3));
            //typeMappings.Add((612, 4));
            //typeMappings.Add((996, 4));

            //IEnumerable<IEnumerable<(ReferenceInt key, ReferenceInt value)>> permutations = typeMappings.GetPermutations();

            foreach (IEnumerable<(ReferenceInt key, ReferenceInt value)> permutation in sequences)
            {
                Add(permutation);
            }
        }
    }
}