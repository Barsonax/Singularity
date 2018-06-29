using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Singularity.Bindings;
using Singularity.Extensions;

namespace Singularity
{
	public class Dependency
	{
		public UnresolvedDependency UnresolvedDependency { get; }
		public ResolvedDependency ResolvedDependency { get; }

		public Dependency(UnresolvedDependency unresolvedDependency, ResolvedDependency resolvedDependency)
		{
			UnresolvedDependency = unresolvedDependency;
			ResolvedDependency = resolvedDependency;
		}
	}

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

	public class ResolvedDependency
	{
		public Expression Expression { get; }

		public ResolvedDependency(Expression expression)
		{
			Expression = expression;
		}
	}
}