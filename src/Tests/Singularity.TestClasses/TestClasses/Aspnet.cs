using System;
using System.Threading;

namespace Singularity.TestClasses.TestClasses
{
    public class TestController1 : IDisposable
    {
        private static int counter;
        private static int disposeCount;

        public TestController1(IRepositoryTransient1 transient1, IRepositoryTransient2 repositoryTransient2, IRepositoryTransient3 repositoryTransient3)
        {
            if (transient1 == null) throw new ArgumentNullException(nameof(transient1));
            if (repositoryTransient2 == null) throw new ArgumentNullException(nameof(repositoryTransient2));
            if (repositoryTransient3 == null) throw new ArgumentNullException(nameof(repositoryTransient3));

            Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get => counter;
            set => counter = value;
        }

        public static int DisposeCount
        {
            get => disposeCount;
            set => disposeCount = value;
        }

        public void Dispose()
        {
            Interlocked.Increment(ref disposeCount);
        }
    }

    public class TestController2 : IDisposable
    {
        private static int counter;
        private static int disposeCount;

        public TestController2(IRepositoryTransient1 transient1, IRepositoryTransient2 repositoryTransient2, IRepositoryTransient3 repositoryTransient3)
        {
            if (transient1 == null) throw new ArgumentNullException(nameof(transient1));
            if (repositoryTransient2 == null) throw new ArgumentNullException(nameof(repositoryTransient2));
            if (repositoryTransient3 == null) throw new ArgumentNullException(nameof(repositoryTransient3));

            Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get => counter;
            set => counter = value;
        }

        public static int DisposeCount
        {
            get => disposeCount;
            set => disposeCount = value;
        }

        public void Dispose()
        {
            Interlocked.Increment(ref disposeCount);
        }
    }

    public class TestController3 : IDisposable
    {
        private static int counter;
        private static int disposeCount;

        public TestController3(IRepositoryTransient1 transient1, IRepositoryTransient2 repositoryTransient2, IRepositoryTransient3 repositoryTransient3)
        {
            if (transient1 == null) throw new ArgumentNullException(nameof(transient1));
            if (repositoryTransient2 == null) throw new ArgumentNullException(nameof(repositoryTransient2));
            if (repositoryTransient3 == null) throw new ArgumentNullException(nameof(repositoryTransient3));

            Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get => counter;
            set => counter = value;
        }

        public static int DisposeCount
        {
            get => disposeCount;
            set => disposeCount = value;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Interlocked.Increment(ref disposeCount);
        }
    }

    public interface IRepositoryTransient1 { }

    public interface IRepositoryTransient2 { }

    public interface IRepositoryTransient3 { }

    public class RepositoryTransient1 : IRepositoryTransient1
    {
        private static int counter;

        public RepositoryTransient1(ISingleton1 singleton, IScopedService scopedService)
        {
            if (singleton == null) throw new ArgumentNullException(nameof(singleton));
            if (scopedService == null) throw new ArgumentNullException(nameof(scopedService));

            Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get => counter;
            set => counter = value;
        }
    }

    public class RepositoryTransient2 : IRepositoryTransient2
    {
        private static int counter;

        public RepositoryTransient2(ISingleton1 singleton, IScopedService scopedService)
        {
            if (singleton == null) throw new ArgumentNullException(nameof(singleton));
            if (scopedService == null) throw new ArgumentNullException(nameof(scopedService));

            Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get => counter;
            set => counter = value;
        }
    }

    public class RepositoryTransient3 : IRepositoryTransient3
    {
        private static int counter;

        public RepositoryTransient3(ISingleton1 singleton, IScopedService scopedService)
        {
            if (singleton == null) throw new ArgumentNullException(nameof(singleton));
            if (scopedService == null) throw new ArgumentNullException(nameof(scopedService));

            Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get => counter;
            set => counter = value;
        }
    }

    public interface IScopedService { }

    public class ScopedService : IScopedService
    {
        private static int counter;

        public ScopedService()
        {
            Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get => counter;
            set => counter = value;
        }
    }
}
