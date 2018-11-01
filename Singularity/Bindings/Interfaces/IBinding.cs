using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Singularity.Enums;

namespace Singularity.Bindings
{
	public interface IBinding
	{
		Type DependencyType { get; }
		Expression Expression { get; }
		Lifetime Lifetime { get; }
		Action<object> OnDeath { get; }
		IReadOnlyList<IDecoratorBinding> Decorators { get; }
	}
}
