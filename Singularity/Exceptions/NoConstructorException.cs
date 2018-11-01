using System;

namespace Singularity.Exceptions
{
    public class NoConstructorException : Exception
    {
        public NoConstructorException(string message) : base(message)
        {
        }
    }
}