using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Singularity.Enums;
using Singularity.Graph;

namespace Singularity.Bindings
{
	internal interface IBinding
	{
		BindingMetadata BindingMetadata { get; }
		Type DependencyType { get; }
		Expression? Expression { get; }
		Lifetime Lifetime { get; }
		Action<object>? OnDeath { get; }
		IReadOnlyList<IDecoratorBinding> Decorators { get; }
	}
}
