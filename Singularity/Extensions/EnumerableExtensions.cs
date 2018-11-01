using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Singularity
{
	public static class EnumerableExtensions
	{
		[DebuggerStepThrough]
		public static bool TryExecute<TObject>(this IEnumerable<TObject> objects, Action<TObject> action, out IList<Exception> exceptions)
		{
			exceptions = null;
			foreach (TObject o in objects)
			{
				try
				{
					action.Invoke(o);
				}
				catch (Exception e)
				{
					if (exceptions == null) exceptions = new List<Exception>();
					exceptions.Add(e);
				}
			}

			return exceptions != null;
		}
	}
}
