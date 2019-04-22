using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Singularity
{
	internal static class EnumerableExtensions
	{
		[DebuggerStepThrough]
		public static bool TryExecute<TObject>(this IEnumerable<TObject> objects, Action<TObject> action, out IList<Exception> exceptions)
		{
			exceptions = null!;
			foreach (TObject o in objects)
			{
				try
				{
					action(o);
				}
				catch (Exception e)
				{
					if (exceptions == null) exceptions = new List<Exception>();
					exceptions.Add(e);
				}
			}

			return exceptions != null;
		}

        public static bool CollectionsAreEqual<T>(this IReadOnlyList<T> list1, IReadOnlyList<T> list2)
        {
            return list1.Count == list2.Count && CollectionsAreEqualInternal(list1, list2);
        }

        public static bool CollectionsAreEqual<T>(this IEnumerable<T> collection1, IEnumerable<T> collection2)
        {
            return CollectionsAreEqualInternal(collection1, collection2);
        }

        private static bool CollectionsAreEqualInternal<T>(this IEnumerable<T> collection1, IEnumerable<T> collection2)
        {
            return collection1.OrderBy(i => i).SequenceEqual(collection2.OrderBy(i => i));
        }
    }
}
