using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Singularity.Exceptions
{
    [Serializable]
    public class RegistrationCollisionException : Exception
    {
        public RegistrationCollisionException(Type[] existingRegistrationTypes, Type[] newRegistrationTypes) : base(GetMessage(existingRegistrationTypes, newRegistrationTypes))
        {

        }

        private static string GetMessage(Type[] existingRegistrationTypes, Type[] newRegistrationTypes)
        {
            string existingTypes = string.Join(", ", newRegistrationTypes.Select(x => x.FullName));
            string newTypes = string.Join(", ", existingRegistrationTypes.Select(x => x.FullName));
            return $"Registration with types {existingTypes} partially collides with existing registration types {newTypes}. If you want to add to a existing collection make sure the types match";
        }

        public RegistrationCollisionException(Type[] existingRegistrationTypes, Type[] newRegistrationTypes, Exception inner) : base(GetMessage(existingRegistrationTypes, newRegistrationTypes), inner)
        {
        }

        protected RegistrationCollisionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
