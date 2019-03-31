using System;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public sealed class BindingConfigException : SingularityException
    {
        internal BindingConfigException(string message) : base(message)
        {
        }

        public BindingConfigException()
        {
        }

        protected BindingConfigException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}