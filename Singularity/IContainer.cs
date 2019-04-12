using System;

namespace Singularity
{
    public interface IContainer : IDisposable
    {
        T GetInstance<T>() where T : class;
        object GetInstance(Type type);
        void MethodInject(object instance);
    }
}