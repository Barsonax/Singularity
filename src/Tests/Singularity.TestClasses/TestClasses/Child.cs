using System;

namespace Singularity.TestClasses.TestClasses
{
    public class ScopedTransient : ITransient1
    {

    }

    public class ScopedCombined1 : ICombined1
    {
        public ITransient1 Transient { get; }
        public ISingleton1 Singleton { get; }
        public ScopedCombined1(ITransient1 transient, ISingleton1 singleton)
        {
            Transient = transient ?? throw new ArgumentNullException(nameof(transient));
            Singleton = singleton ?? throw new ArgumentNullException(nameof(singleton));
        }

    }

    public class ScopedCombined2 : ICombined2
    {
        public ITransient2 Transient { get; }
        public ISingleton2 Singleton { get; }
        public ScopedCombined2(ITransient2 transient, ISingleton2 singleton)
        {
            Transient = transient ?? throw new ArgumentNullException(nameof(transient));
            Singleton = singleton ?? throw new ArgumentNullException(nameof(singleton));
        }
    }

    public class ScopedCombined3 : ICombined3
    {
        public ITransient3 Transient { get; }
        public ISingleton3 Singleton { get; }
        public ScopedCombined3(ITransient3 transient, ISingleton3 singleton)
        {
            Transient = transient ?? throw new ArgumentNullException(nameof(transient));
            Singleton = singleton ?? throw new ArgumentNullException(nameof(singleton));
        }
    }
}
