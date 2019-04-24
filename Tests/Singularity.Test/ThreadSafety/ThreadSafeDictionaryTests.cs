using System.Collections.Generic;
using System.Linq.Expressions;
using Singularity.Collections;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.ThreadSafety
{
    public class ThreadSafeDictionaryTests
    {
        [Fact]
        public void AddAndGet()
        {
            var testCases = new List<(ReferenceInt input, object output)>();

            for (var i = 0; i < 100; i++)
            {
                testCases.Add((new ReferenceInt(i), new object()));
            }

            var dic = new ThreadSafeDictionary<ReferenceInt, object>();

            var tester = new ThreadSafetyTester<(ReferenceInt input, object expectedOutput)>(() => testCases);

            tester.Test(testCase =>
            {
                dic.Add(testCase.input, testCase.expectedOutput);
                object output = dic.Get(testCase.input);
                Assert.Equal(testCase.expectedOutput, output);
            });

            Assert.Equal(testCases.Count * tester.TaskCount, dic.Count);
        }
    }
}
