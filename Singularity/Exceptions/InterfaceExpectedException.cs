using System;

namespace Singularity
{
    public class InterfaceExpectedException : Exception
    {
        public InterfaceExpectedException(string message) : base(message)
        {
        }
    }
}