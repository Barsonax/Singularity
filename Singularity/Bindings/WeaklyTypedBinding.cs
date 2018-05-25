using System;
using System.Collections.Generic;

namespace Singularity.Bindings
{
	public class WeaklyTypedBinding : IBinding
	{
		public Type DependencyType { get; }
		public IConfiguredBinding ConfiguredBinding { get; }
		public IReadOnlyList<IDecoratorBinding> Decorators { get; }

		public WeaklyTypedBinding(Type dependencyType, IConfiguredBinding configuredBinding, IReadOnlyList<IDecoratorBinding> decorators)
		{
			DependencyType = dependencyType;
			ConfiguredBinding = configuredBinding;
			Decorators = decorators;
		}
	}
}