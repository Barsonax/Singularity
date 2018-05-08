using System;

namespace Singularity.Test.TestClasses
{
    public class Disposable : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
