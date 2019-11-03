using System.Collections.Generic;
using Singularity.Collections;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.ThreadSafety
{
    public class DisposeListThreadSafetyTests
    {
        [Fact]
        public void DisposeList_Add()
        {
            var list = new ActionList<TrackableDisposable>(obj => obj.Dispose());
            var trackers = new List<Tracker>();

            var tester = new ThreadSafetyTester<TrackableDisposable>(() =>
            {
                var tracker = new Tracker();
                var cases = new List<TrackableDisposable>();
                lock (trackers)
                {
                    trackers.Add(tracker);
                }

                for (var i = 0; i < 100; i++)
                {
                    cases.Add(new TrackableDisposable(tracker));
                }

                return cases;
            });

            tester.Test(testCase =>
            {
                list.Add(testCase);
            });

            list.Invoke();
            foreach (Tracker tracker in trackers)
            {
                Assert.Equal(100, tracker.DisposeCount);
            }
        }
    }
}
