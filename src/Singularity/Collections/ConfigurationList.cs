using System;
using System.Collections;
using System.Collections.Generic;

namespace Singularity.Collections
{
    /// <summary>
    /// A list with some utility methods for configuration purposes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConfigurationList<T> : IEnumerable<T>
    {
        private List<T> Elements { get; } = new List<T>();

        /// <summary>
        /// Appends the <paramref name="element"/>
        /// </summary>
        /// <param name="element"></param>
        public void Add(T element)
        {
            Elements.Add(element);
        }

        /// <summary>
        /// Removes the first element that matches with the <paramref name="predicate"/>
        /// </summary>
        /// <param name="predicate"></param>
        public void Remove(Func<T, bool> predicate)
        {
            for (var i = 0; i < Elements.Count; i++)
            {
                if (predicate(Elements[i]))
                {
                    Elements.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// Replaces the first element that matches with the <paramref name="predicate"/> with <paramref name="element"/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="element"></param>
        /// <exception cref="ArgumentException">If no element matches</exception>
        public void Replace(Func<T, bool> predicate, T element)
        {
            for (var i = 0; i < Elements.Count; i++)
            {
                if (predicate(Elements[i]))
                {
                    Elements[i] = element;
                    return;
                }
            }
            throw new ArgumentException($"No element matched with the predicate {predicate}");
        }

        /// <summary>
        /// Inserts the <paramref name="element"/> before the first element that matches with the <paramref name="predicate"/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="element"></param>
        /// <exception cref="ArgumentException">If no element matches</exception>
        public void Before(Func<T, bool> predicate, T element)
        {
            for (var i = 0; i < Elements.Count; i++)
            {
                if (predicate(Elements[i]))
                {
                    Elements.Insert(i, element);
                    return;
                }
            }
            throw new ArgumentException($"No element matched with the predicate {predicate}");
        }

        /// <summary>
        /// Inserts the <paramref name="element"/> after the first element that matches with the <paramref name="predicate"/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="element"></param>
        /// <exception cref="ArgumentException">If no element matches</exception>
        public void After(Func<T, bool> predicate, T element)
        {
            for (var i = 0; i < Elements.Count; i++)
            {
                if (predicate(Elements[i]))
                {
                    Elements.Insert(i + 1, element);
                    return;
                }
            }
            throw new ArgumentException($"No element matched with the predicate {predicate}");
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => Elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
