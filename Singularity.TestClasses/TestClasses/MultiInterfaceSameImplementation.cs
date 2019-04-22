using System;
using System.Collections.Generic;
using System.Text;

namespace Singularity.TestClasses.TestClasses
{
    public class Implementation1 : IService1, IService2, IService3
    {
    }

    public interface IService1 { }
    public interface IService2 { }
    public interface IService3 { }

}
