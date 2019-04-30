using System;

namespace Singularity.TestClasses.TestClasses
{
    public interface ICombined1
    {
        ITransient1 Transient { get; }
        ISingleton1 Singleton { get; }
    }

    public interface ICombined2
    {
        ITransient2 Transient { get; }
        ISingleton2 Singleton { get; }
    }

    public interface ICombined3
    {
        ITransient3 Transient { get; }
        ISingleton3 Singleton { get; }
    }

    public sealed class Combined1 : ICombined1
    {
        public ITransient1 Transient { get; }
        public ISingleton1 Singleton { get; }

        public Combined1(ISingleton1 singleton, ITransient1 transient)
        {
            Transient = transient ?? throw new ArgumentNullException(nameof(transient));
            Singleton = singleton ?? throw new ArgumentNullException(nameof(singleton));
        }
    }

    public abstract class Combined2 : ICombined2
    {
        public ITransient2 Transient { get; }
        public ISingleton2 Singleton { get; }

        public Combined2(ISingleton2 singleton, ITransient2 transient)
        {
            Transient = transient ?? throw new ArgumentNullException(nameof(transient));
            Singleton = singleton ?? throw new ArgumentNullException(nameof(singleton));
        }
    }

    public abstract class Combined3 : ICombined3
    {
        public ITransient3 Transient { get; }
        public ISingleton3 Singleton { get; }

        public Combined3(ISingleton3 singleton, ITransient3 transient)
        {
            Transient = transient ?? throw new ArgumentNullException(nameof(transient));
            Singleton = singleton ?? throw new ArgumentNullException(nameof(singleton));
        }
    }
}
