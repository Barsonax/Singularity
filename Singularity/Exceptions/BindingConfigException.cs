using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    /// <summary>
    /// Exception for when something is wrong with the bindings.
    /// </summary>
    [Serializable]
    public class BindingConfigException : SingularityException
    {
        internal BindingConfigException(string message, Exception? inner = null) : base(message, inner)
        {
        }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected BindingConfigException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}