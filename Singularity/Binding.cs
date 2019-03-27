using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Singularity.Bindings;
using Singularity.Graph;

namespace Singularity
{
    public readonly struct Binding
    {
        public BindingMetadata BindingMetadata { get; }
        public Type DependencyType { get; }
        public Expression? Expression { get; }
        public CreationMode CreationMode { get; }
        public Action<object>? OnDeathAction { get; }
        public Expression[] Decorators { get; }

        public Binding(BindingMetadata bindingMetadata, Type dependencyType, Expression? expression, CreationMode creationMode, Expression[] decorators, Action<object>? onDeath)
        {
            BindingMetadata = bindingMetadata ?? throw new ArgumentNullException(nameof(bindingMetadata));
            DependencyType = dependencyType ?? throw new ArgumentNullException(nameof(dependencyType));
            CreationMode = creationMode;
            Expression = expression;
            Decorators = decorators ?? throw new ArgumentNullException(nameof(decorators));
            OnDeathAction = onDeath;
        }

        public Binding(WeaklyTypedBinding binding) : this(binding.BindingMetadata, binding.DependencyType, binding.Expression, binding.CreationMode, 
            binding.Decorators != null ? binding.Decorators.Select(x => x.Expression!).ToArray() : new Expression[0], binding.OnDeathAction)
        {
        }
    }
}
