using System;

namespace Singularity.TestClasses.TestClasses
{
    public class Implementation1 : IService1, IService2, IService3
    {
    }

    public interface IService1 { }
    public interface IService2 { }

    public class Service2Decorator : IService2
    {
        public readonly IService2 Service;
        public Service2Decorator(IService2 service)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }
    }
    public interface IService3 { }

}
