using System;
using System.Linq.Expressions;

namespace Singularity
{
	public interface IBinding
	{
		Type DependencyType { get; }
		Expression Expression { get; set; }
		Lifetime Lifetime { get; }
	}
}
