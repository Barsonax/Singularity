using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Singularity.Collections;
using Singularity.Exceptions;

namespace Singularity
{
    internal static class ServiceTypeValidator
    {
        public static void CheckIsEnumerable(Type dependencyType)
        {
            if (IsEnumerable(dependencyType))
                ThrowEnumerableRegistrationException(dependencyType);
        }

        public static void CheckIsAssignable(Type dependencyType, Type instanceType)
        {
            if (dependencyType.ContainsGenericParameters)
            {
                if (!instanceType.ContainsGenericParameters || instanceType.GenericTypeArguments.Length != dependencyType.GenericTypeArguments.Length)
                {
                    throw new TypeNotAssignableException($"Open generic type {dependencyType} is not implemented by {instanceType}");
                }
            }
            else
            {
                if (!dependencyType.IsAssignableFrom(instanceType)) throw new TypeNotAssignableException($"{dependencyType} is not implemented by {instanceType}");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckIsAssignable(SinglyLinkedListNode<Type> dependencyTypes, Type instanceType)
        {
            foreach (Type dependencyType in dependencyTypes)
            {
                CheckIsAssignable(dependencyType, instanceType);
            }
        }

        public static void ThrowEnumerableRegistrationException(Type dependencyType)
        {
            throw new EnumerableRegistrationException($"don't register {dependencyType} as IEnumerable directly. Instead register them as you would normally.");
        }

        public static bool IsEnumerable(Type dependencyType)
        {
            if (dependencyType.IsGenericType && dependencyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return true;
            }

            return false;
        }

        internal class Cache<T>
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