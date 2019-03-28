using System;

namespace Singularity.Collections
{
    internal struct ImmutableList<T>
    {
        public struct Utils
        {
            public static readonly ImmutableList<T> Empty = new ImmutableList<T>(new T[0]);
        }

        public readonly T[] Items;

        private ImmutableList(ImmutableList<T> previousList, T value)
        {
            Items = new T[previousList.Items.Length + 1];
            Array.Copy(previousList.Items, Items, previousList.Items.Length);
            Items[Items.Length - 1] = value;
        }

        private ImmutableList(T[] items)
        {
            Items = items;
        }

        public ImmutableList<T> Add(T value)
        {
            return new ImmutableList<T>(this, value);
        }
    }


}
