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

    public class DisposableDecorator : IDisposable
    {
        public IDisposable Disposable { get; }

        public DisposableDecorator(IDisposable disposable)
        {
            Disposable = disposable;
        }

        public void Dispose()
        {
            Disposable.Dispose();
        }
    }
}
