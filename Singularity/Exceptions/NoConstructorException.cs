using System;

namespace Singularity
{
    public class NoConstructorException : Exception
    {
        public NoConstructorException(string message) : base(message)
        {
        }
    }
}