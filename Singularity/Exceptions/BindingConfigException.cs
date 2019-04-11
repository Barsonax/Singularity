using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public class BindingConfigException : SingularityException
    {
        public BindingConfigException()
        {
        }

        internal BindingConfigException(string message) : base(message)
        {
        }

        public BindingConfigException(string message, Exception inner) : base(message, inner)
        {
        }

        protected BindingConfigException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}