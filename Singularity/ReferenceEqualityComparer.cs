using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Singularity
{
	internal sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T> where T : class
	{
		internal static readonly ReferenceEqualityComparer<T> Instance = new ReferenceEqualityComparer<T>();
		public bool Equals(T x, T y) => ReferenceEquals(x, y);
		public int GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj);
	}
}