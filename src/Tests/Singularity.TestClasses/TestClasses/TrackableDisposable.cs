using System;

namespace Singularity.TestClasses.TestClasses
{
    public class TrackableDisposable : IDisposable
    {
        private readonly ITracker _tracker;
        public bool IsDisposed { get; private set; }
        public TrackableDisposable(ITracker tracker)
        {
            _tracker = tracker;
        }

        public void Dispose()
        {
            if (IsDisposed) throw new InvalidOperationException("Is already disposed!");
            IsDisposed = true;
            _tracker.Increment();
        }
    }
}