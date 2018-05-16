using System;
using System.Collections.Generic;

namespace Singularity.Bindings
{
	public class WeaklyTypedBinding : IBinding
	{
		public Type DependencyType { get; }
		public IConfiguredBinding ConfiguredBinding { get; }
		public List<IDecoratorBinding> Decorators { get; }

		public WeaklyTypedBinding(Type dependencyType, IConfiguredBinding configuredBinding, List<IDecoratorBinding> decorators)
		{
			DependencyType = dependencyType;
			ConfiguredBinding = configuredBinding;
			Decorators = decorators;
		}
	}
}