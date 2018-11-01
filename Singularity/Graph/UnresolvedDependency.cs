using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Singularity.Bindings;
using Singularity.Enums;

namespace Singularity.Graph
{
    public sealed class UnresolvedDependency
	{
		public Type DependencyType { get; }
		public Expression Expression { get; }
		public Lifetime Lifetime { get; }
		public Action<object> OnDeath { get; }
		public IReadOnlyCollection<IDecoratorBinding> Decorators { get; }

		public UnresolvedDependency(Type dependencyType, Expression expression, Lifetime lifetime, IReadOnlyCollection<IDecoratorBinding> decorators, Action<object> onDeath)
		{
			DependencyType = dependencyType;
			Lifetime = lifetime;
			Expression = expression;
			Decorators = decorators;
			OnDeath = onDeath;
		}
	}
}