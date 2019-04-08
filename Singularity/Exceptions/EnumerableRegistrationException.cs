using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public class EnumerableRegistrationException : Exception
    {

        public EnumerableRegistrationException()
        {
        }

        public EnumerableRegistrationException(string message) : base(message)
        {
        }

        public EnumerableRegistrationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected EnumerableRegistrationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}