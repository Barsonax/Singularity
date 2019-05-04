using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    /// <summary>
    /// Exception for when a incorrect enum value is passed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public sealed class InvalidEnumValueException<T> : SingularityException
        where T : struct, Enum
    {
        internal InvalidEnumValueException(T enumValue, Exception? inner = null) : base(GetMessage(enumValue), inner)
        {
        }

        private static string GetMessage(T enumValue)
        {
            return $"{enumValue} is a invalid value, valid values are: {string.Join(", ", EnumMetadata<T>.Values)}";
        }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private InvalidEnumValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}