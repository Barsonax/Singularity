using Singularity.Test.TestClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Singularity.Test.ThreadSafety
{
    public class ContainerThreadSafetyTests
    {
        [Fact]
        public void GetInstance()
        {
            var config = new BindingConfig();

            var registrations = new List<(Type abstractType, Type concreteType)>();

            registrations.Add((typeof(ITestService10), typeof(TestService10)));
            registrations.Add((typeof(ITestService11), typeof(TestService11)));
            registrations.Add((typeof(ITestService12), typeof(TestService12)));

            registrations.Add((typeof(ITestService20), typeof(TestService20)));
            registrations.Add((typeof(ITestService21), typeof(TestService21)));
            registrations.Add((typeof(ITestService22), typeof(TestService22)));

            registrations.Add((typeof(ITestService30), typeof(TestService30)));
            registrations.Add((typeof(ITestService31), typeof(TestService31)));
            registrations.Add((typeof(ITestService32), typeof(TestService32)));

            var disposeCount = 0;
            foreach ((Type abstractType, Type concreteType) registration in registrations)
            {
                config.Register(registration.abstractType, registration.concreteType).OnDeath(obj => disposeCount++);
            }

            for (int iteration = 0; iteration < 100; iteration++)
            {
                disposeCount = 0;
                var container = new Container(config);

                var manualResetEvent = new ManualResetEvent(false);
                var tasks = new Task[1000];
                for (var i = 0; i < tasks.Length; i++)
                {
                    int seed = i;
                    tasks[i] = Task.Run(() =>
                    {
                        //Randomize the order to maximize the chance on threading issues.
                        var testCases = Shuffle(registrations, seed);

                        manualResetEvent.WaitOne();
                        foreach ((Type abstractType, Type concreteType) registration in testCases)
                        {
                            object concreteType = container.GetInstance(registration.abstractType);
                            Assert.Equal(registration.concreteType, concreteType.GetType());
                        }
                    });
                }

                //Unleash the hammer
                manualResetEvent.Set();
                Task.WaitAll(tasks);
                container.Dispose();
                Assert.Equal(18000,disposeCount);
            }
        }

        public static List<T> Shuffle<T>(List<T> list, int seed)
        {
            var rng = new Random(seed);
            var randomizedList = list.ToList();
            int n = randomizedList.Count;
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
