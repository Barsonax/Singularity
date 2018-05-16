using System;
using System.Collections.Generic;

namespace Singularity.Bindings
{
	public interface IBinding
	{
		Type DependencyType { get; }
		IConfiguredBinding ConfiguredBinding { get; }
		List<IDecoratorBinding> Decorators { get; }
	}
}
