using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Singularity.Test.ThreadSafety
{
    /// <summary>
    /// A helper class to search for threading bugs
    /// Runs a bunch of tasks at the same time that execute testcases in a random order
    /// </summary>
    /// <typeparam name="TTestCase"></typeparam>
    public class ThreadSafetyTester<TTestCase>
    {
        public int TaskCount { get; } = 10;
        private readonly Func<IEnumerable<TTestCase>> _testCasesFactory;

        public ThreadSafetyTester(Func<IEnumerable<TTestCase>> testCasesFactory)
        {
            _testCasesFactory = testCasesFactory;
        }

        public void Test(Action<TTestCase> action)
        {
            var manualResetEvent = new ManualResetEvent(false);
            var tasks = new Task[TaskCount];
            var threadsWaiting = 0;

            for (var i = 0; i < tasks.Length; i++)
            {
                int seed = i;
                tasks[i] = Task.Run(() =>
                {
                    //Randomize the order to maximize the chance on threading issues.
                    TTestCase[] testCases = Shuffle(_testCasesFactory.Invoke(), seed);

                    Interlocked.Increment(ref threadsWaiting);
                    manualResetEvent.WaitOne();

                    foreach (TTestCase testCase in testCases)
                    {
                        action.Invoke(testCase);
                    }
                });
            }

            while (threadsWaiting != tasks.Length)
            {
                //Wait till all threads are ready
            }

            //Unleash the hammer
            manualResetEvent.Set();

            Task.WaitAll(tasks);
        }

        private static T[] Shuffle<T>(IEnumerable<T> list, int seed)
        {
            var rng = new Random(seed);
            T[] randomizedList = list.ToArray();
            int n = randomizedList.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = randomizedList[k];
                randomizedList[k] = randomizedList[n];
                randomizedList[n] = value;
            }

            return randomizedList;
        }
    }
}