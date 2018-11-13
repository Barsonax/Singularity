using System;

namespace Singularity.Exceptions
{
    public sealed class InterfaceExpectedException : SingularityException
    {
	    internal InterfaceExpectedException(string message) : base(message)
        {
        }
    }
}