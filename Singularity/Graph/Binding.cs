using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Singularity.Enums;

namespace Singularity.Graph
{
    public sealed class Binding
    {
        public BindingMetadata BindingMetadata { get; }
        public Type DependencyType { get; }
        public Expression? Expression { get; }
        public Lifetime Lifetime { get; }
        public Action<object>? OnDeath { get; }
        public IReadOnlyList<DecoratorBinding> Decorators { get; }

        public Binding(BindingMetadata bindingMetadata, Type dependencyType, Expression? expression, Lifetime lifetime, IReadOnlyList<DecoratorBinding> decorators, Action<object>? onDeath)
        {
            BindingMetadata = bindingMetadata ?? throw new ArgumentNullException(nameof(bindingMetadata));
            DependencyType = dependencyType ?? throw new ArgumentNullException(nameof(dependencyType));
            Lifetime = lifetime;
            Expression = expression;
            Decorators = decorators ?? throw new ArgumentNullException(nameof(decorators));
            OnDeath = onDeath;
        }
    }

    public sealed class DecoratorBinding
    {
        public Expression Expression { get; }

        public DecoratorBinding(Expression expression)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }
    }
}
