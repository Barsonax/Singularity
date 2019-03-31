using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public sealed class InvalidLifetimeException : SingularityException
    {
        internal InvalidLifetimeException(CreationMode creationMode) : base($"{creationMode} is a invalid value, valid values are: {string.Join(", ", EnumMetadata<CreationMode>.Values)}")
        {

        }


        public InvalidLifetimeException()
        {
        }

        protected InvalidLifetimeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}