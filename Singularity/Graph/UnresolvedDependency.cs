using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Singularity.Enums;

namespace Singularity.Graph
{
    public sealed class UnresolvedDependency
    {
        public BindingMetadata BindingMetadata { get; }
        public Type DependencyType { get; }
        public Expression Expression { get; }
        public Lifetime Lifetime { get; }
        public Action<object>? OnDeath { get; }
        public IReadOnlyCollection<DecoratorBinding> Decorators { get; }

        public UnresolvedDependency(BindingMetadata bindingMetadata, Type dependencyType, Expression expression, Lifetime lifetime, IReadOnlyCollection<DecoratorBinding> decorators, Action<object>? onDeath)
        {
            BindingMetadata = bindingMetadata ?? throw new ArgumentNullException(nameof(bindingMetadata));
            DependencyType = dependencyType ?? throw new ArgumentNullException(nameof(dependencyType));
            Lifetime = lifetime;
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            Decorators = decorators ?? throw new ArgumentNullException(nameof(decorators));
            OnDeath = onDeath;
        }
    }
}