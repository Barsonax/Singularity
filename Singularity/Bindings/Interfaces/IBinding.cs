using System;
using System.Linq.Expressions;
using Singularity.Bindings;

namespace Singularity
{
	public interface IBinding
	{
		Type DependencyType { get; }
		IConfiguredBinding ConfiguredBinding { get; }
	}
}
