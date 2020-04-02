using System;

namespace Singularity.TestClasses.TestClasses
{
    public class Implementation1 : IService1, IService2, IService3, IService4
    {
    }

    public class Implementation2 : IService1, IService2, IService3, IService4
    {
    }

    public interface IService1 { }
    public interface IService2 { }
    public interface IService3 { }
    public interface IService4 { }

    public class Service1Decorator : IService1
    {
        public readonly IService1 Service;
        public Service1Decorator(IService1 service)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }
    }

    public class Service2Decorator : IService2
    {
        public readonly IService2 Service;
        public Service2Decorator(IService2 service)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }
    }
}
