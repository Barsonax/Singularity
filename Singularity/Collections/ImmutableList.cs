using System;

namespace Singularity.Collections
{
    internal readonly struct ImmutableList<T>
    {
        public struct Utils
        {
            public static readonly ImmutableList<T> Empty = new ImmutableList<T>(new T[0]);
        }

        public readonly T[] Items;

        private ImmutableList(ImmutableList<T> previousList, in T value)
        {
            Items = new T[previousList.Items.Length + 1];
            for (var i = 0; i < previousList.Items.Length; i++)
            {
                Items[i] = previousList.Items[i];
            }

            Items[Items.Length - 1] = value;
        }

        private ImmutableList(T[] items)
        {
            Items = items;
        }

        public ImmutableList<T> Add(in T value)
        {
            return new ImmutableList<T>(this, in value);
        }
    }


}
