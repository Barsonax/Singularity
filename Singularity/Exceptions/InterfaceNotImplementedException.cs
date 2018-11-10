using System;

namespace Singularity.Exceptions
{
    public sealed class InterfaceNotImplementedException : SingularityException
    {
        internal InterfaceNotImplementedException(string message) : base(message)
        {
        }
    }
}