using System;

namespace Singularity
{
    public class CannotAutoResolveConstructorException : Exception
    {
        public CannotAutoResolveConstructorException(string message) : base(message)
        {
        }
    }
}