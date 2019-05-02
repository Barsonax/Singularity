using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Exceptions
{
    [Serializable]
    public class AbstractTypeResolveException : Exception
    {
        public AbstractTypeResolveException(string message) : base(message)
        {
        }

        public AbstractTypeResolveException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AbstractTypeResolveException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }


}
