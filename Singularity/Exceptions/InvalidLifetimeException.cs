using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public class InvalidLifetimeException : SingularityException
    {
        public InvalidLifetimeException()
        {
        }

        public InvalidLifetimeException(CreationMode creationMode) : base(GetMessage(creationMode))
        {

        }

        public InvalidLifetimeException(CreationMode creationMode, Exception inner) : base(GetMessage(creationMode), inner)
        {
        }

        private static string GetMessage(CreationMode creationMode)
        {
            return $"{creationMode} is a invalid value, valid values are: {string.Join(", ", EnumMetadata<CreationMode>.Values)}";
        }

        protected InvalidLifetimeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}