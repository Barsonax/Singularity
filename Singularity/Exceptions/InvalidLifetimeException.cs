using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public class InvalidLifetimeException : SingularityException
    {
        internal InvalidLifetimeException(CreationMode creationMode, Exception? inner = null) : base(GetMessage(creationMode), inner)
        {
        }

        private static string GetMessage(CreationMode creationMode)
        {
            return $"{creationMode} is a invalid value, valid values are: {string.Join(", ", EnumMetadata<CreationMode>.Values)}";
        }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected InvalidLifetimeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}