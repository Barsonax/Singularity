using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Singularity.Collections;
using Singularity.Exceptions;

namespace Singularity
{
    internal static class ServiceTypeValidator
    {
        public static void CheckIsEnumerable(Type serviceType)
        {
            if (IsEnumerable(serviceType))
                ThrowEnumerableRegistrationException(serviceType);
        }

        public static void CheckIsAssignable(Type serviceType, Type implementationType)
        {
            if (serviceType.ContainsGenericParameters)
            {
                if (!implementationType.ContainsGenericParameters || implementationType.GenericTypeArguments.Length != serviceType.GenericTypeArguments.Length)
                {
                    throw new TypeNotAssignableException($"Open generic type {serviceType} is not implemented by {implementationType}");
                }
            }
            else
            {
                if (!serviceType.IsAssignableFrom(implementationType)) throw new TypeNotAssignableException($"{serviceType} is not implemented by {implementationType}");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckIsAssignable(SinglyLinkedListNode<Type> serviceTypes, Type implementationType)
        {
            foreach (Type serviceType in serviceTypes)
            {
                CheckIsAssignable(serviceType, implementationType);
            }
        }

        private static void ThrowEnumerableRegistrationException(Type serviceType)
        {
            throw new EnumerableRegistrationException($"don't register {serviceType} as IEnumerable directly. Instead register them as you would normally.");
        }

        private static bool IsEnumerable(Type serviceType)
        {
            if (serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return true;
            }

            return false;
        }

        internal static class Cache<T>
        {
            private static readonly bool IsEnumerableCache = IsEnumerable(typeof(T));

            public static void CheckIsEnumerable()
            {
                if (IsEnumerableCache)
                    ThrowEnumerableRegistrationException(typeof(T));
            }
        }
    }
}