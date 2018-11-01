using System;

namespace Singularity.Exceptions
{
    public class InterfaceNotImplementedException : Exception
    {
        public InterfaceNotImplementedException(string message) : base(message)
        {
        }
    }
}