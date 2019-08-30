using System;

namespace Singularity.TestClasses.TestClasses
{
    public class CustomWrapper<T>
        where T : class
    {
        public T Instance { get; }
        public CustomWrapper(T instance)
        {
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }
    }
}
