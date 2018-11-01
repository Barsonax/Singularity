using System;

namespace Singularity.Exceptions
{
    public class InterfaceExpectedException : Exception
    {
        public InterfaceExpectedException(string message) : base(message)
        {
        }
    }
}