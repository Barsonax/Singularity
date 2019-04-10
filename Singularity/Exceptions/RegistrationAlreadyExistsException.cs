using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public class RegistrationAlreadyExistsException : SingularityException
    {
        public RegistrationAlreadyExistsException()
        {
        }

        public RegistrationAlreadyExistsException(string message) : base(message)
        {
        }

        public RegistrationAlreadyExistsException(string message, Exception inner) : base(message, inner)
        {
        }

        protected RegistrationAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}