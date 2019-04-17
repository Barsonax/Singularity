using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace Singularity.Test.ThreadSafety
{
    public class DisposeListThreadSafetyTests
    {
        [Fact]
        public void DisposeList_Add()
        {
            var list = new DisposeList<TrackableDisposable>(obj => obj.Dispose());
            var trackers = new List<DisposeTracker>();

            var tester = new ThreadSafetyTester<TrackableDisposable>(() =>
            {
                var tracker = new DisposeTracker();
                var cases = new List<TrackableDisposable>();
                trackers.Add(tracker);
                for (var i = 0; i < 1000; i++)
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
            foreach (DisposeTracker tracker in trackers)
            {
                Assert.Equal(1000, tracker.DisposeCount);
            }
        }

        public class TrackableDisposable
        {
            private readonly DisposeTracker _tracker;
            public bool IsDisposed { get; private set; }
            public TrackableDisposable(DisposeTracker tracker)
            {
                _tracker = tracker;
            }

            public void Dispose()
            {
                if(IsDisposed) throw new InvalidOperationException("Is already disposed!");
                IsDisposed = true;
                Interlocked.Increment(ref _tracker.DisposeCount);
            }
        }

        public class DisposeTracker
        {
            public int DisposeCount;
        }
    }
}
