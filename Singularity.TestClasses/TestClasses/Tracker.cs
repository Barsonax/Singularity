using System.Threading;

namespace Singularity.TestClasses.TestClasses
{
    public class Tracker : ITracker
    {
        private int _disposeCount;
        public int DisposeCount => _disposeCount;

        public void Increment()
        {
            Interlocked.Increment(ref _disposeCount);
        }
    }

    public interface ITracker
    {
        void Increment();
    }
}