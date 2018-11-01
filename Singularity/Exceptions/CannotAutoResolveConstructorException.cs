using System;

namespace Singularity.Exceptions
{
    public class CannotAutoResolveConstructorException : Exception
    {
        public CannotAutoResolveConstructorException(string message) : base(message)
        {
        }
    }
}