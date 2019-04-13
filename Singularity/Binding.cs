using System;
using System.Diagnostics;
using System.Linq.Expressions;
using Singularity.Bindings;
using Singularity.Graph;

namespace Singularity
{
    [DebuggerDisplay("{Expression?.Type}")]
    internal class Binding
    {
        public BindingMetadata BindingMetadata { get; }
        public Expression? Expression { get; }

        public Lifetime Lifetime { get; }
        public Action<object>? OnDeathAction { get; }

        public Binding(BindingMetadata bindingMetadata, Expression? expression, Lifetime lifetime, Action<object>? onDeath)
        {
            BindingMetadata = bindingMetadata ?? throw new ArgumentNullException(nameof(bindingMetadata));
            Lifetime = lifetime;
            Expression = expression;

            OnDeathAction = onDeath;
        }

        public Binding(WeaklyTypedBinding binding) : this(binding.BindingMetadata, binding.Expression, binding.Lifetime, binding.OnDeathAction)
        {
        }
    }
}
