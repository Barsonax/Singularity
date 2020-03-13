using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    /// <summary>
    /// The base exception class for singularity exceptions.
    /// </summary>
    [Serializable]
	public class SingularityException : Exception
	{
         internal SingularityException(string message, Exception? innerException = null) : base(message, innerException) { }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SingularityException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}