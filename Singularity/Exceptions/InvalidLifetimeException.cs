using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public class InvalidLifetimeException : SingularityException
    {
        internal InvalidLifetimeException(Lifetime lifetime, Exception? inner = null) : base(GetMessage(lifetime), inner)
        {
        }

        private static string GetMessage(Lifetime lifetime)
        {
            return $"{lifetime} is a invalid value, valid values are: {string.Join(", ", EnumMetadata<Lifetime>.Values)}";
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