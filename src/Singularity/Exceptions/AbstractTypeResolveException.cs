using System;
using System.Runtime.Serialization;
using Singularity.Expressions;

namespace Singularity.Exceptions
{
    /// <summary>
    /// Exception thrown when trying to create a instance for a <see cref="AbstractBindingExpression"/>
    /// </summary>
    [Serializable]
    public sealed class AbstractTypeResolveException : Exception
    {
        internal AbstractTypeResolveException(string message, Exception? inner = null) : base(message, inner)
        {
        }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private AbstractTypeResolveException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
