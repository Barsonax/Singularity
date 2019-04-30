using System.Collections;
using System.Collections.Generic;

namespace Singularity.Collections
{
    internal sealed class ArrayList<T> : IEnumerable<T>
    {
        public static readonly T[] Empty = new T[0];
        public T[] Array { get; private set; }

        public ArrayList(T[] values)
        {
            Array = values;
        }

        public ArrayList()
        {
            Array = Empty;
        }

        public void Add(T obj)
        {
            var newArray = new T[Array.Length + 1];

            for (var i = 0; i < Array.Length; i++)
            {
                newArray[i] = Array[i];
            }

            newArray[newArray.Length - 1] = obj;

            Array = newArray;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T value in Array)
            {
                yield return value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
