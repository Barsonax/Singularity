using System;

namespace Singularity
{
    public class InterfaceNotImplementedException : Exception
    {
        public InterfaceNotImplementedException(string message) : base(message)
        {
        }
    }
}