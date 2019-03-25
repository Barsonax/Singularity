using System;
using System.Linq.Expressions;
using Singularity.Graph;

namespace Singularity
{
    public readonly struct Binding
    {
        public BindingMetadata BindingMetadata { get; }
        public Type DependencyType { get; }
        public Expression? Expression { get; }
        public ILifetime Lifetime { get; }
        public Action<object>? OnDeath { get; }
        public Expression[] Decorators { get; }

        public Binding(BindingMetadata bindingMetadata, Type dependencyType, Expression? expression, ILifetime lifetime, Expression[] decorators, Action<object>? onDeath)
        {
            BindingMetadata = bindingMetadata ?? throw new ArgumentNullException(nameof(bindingMetadata));
            DependencyType = dependencyType ?? throw new ArgumentNullException(nameof(dependencyType));
            Lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
            Expression = expression;
            Decorators = decorators ?? throw new ArgumentNullException(nameof(decorators));
            OnDeath = onDeath;
        }
    }
}
