using System.Collections;
using System.Collections.Generic;

namespace Singularity.Collections
{
    /// <summary>
    /// A simple generic array list that allocates a new array for every new element that is added.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ArrayList<T> : IEnumerable<T>
    {
        public static readonly T[] Empty = new T[0];

        /// <summary>
        /// The wrapped array.
        /// </summary>
        public T[] Array { get; private set; }

        public ArrayList(T[] values)
        {
            Array = values;
        }

        public ArrayList()
        {
            Array = Empty;
        }

        /// <summary>
        /// Adds a new element to the array.
        /// Allocates a new array on every call.
        /// </summary>
        /// <param name="obj"></param>
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

        /// <summary>
        /// Returns the last element in the array.
        /// </summary>
        /// <returns></returns>
        public T Last()
        {
            return Array[Array.Length - 1];
        }

        /// <inheritdoc />
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
