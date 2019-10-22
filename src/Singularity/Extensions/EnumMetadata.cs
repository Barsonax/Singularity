﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Singularity
{
    internal static class EnumMetadata<T>
        where T : struct, Enum
    {
        public static readonly ReadOnlyCollection<T> Values = new ReadOnlyCollection<T>(Enum.GetValues(typeof(T)).OfType<T>().ToArray());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidValue(T value)
        {
            return Values.Contains(value);
        }
    }
}