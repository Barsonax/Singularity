using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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

        public CreationMode CreationMode { get; }
        public Action<object>? OnDeathAction { get; }

        public Binding(BindingMetadata bindingMetadata, Expression? expression, CreationMode creationMode, Action<object>? onDeath)
        {
            BindingMetadata = bindingMetadata ?? throw new ArgumentNullException(nameof(bindingMetadata));
            CreationMode = creationMode;
            Expression = expression;

            OnDeathAction = onDeath;
        }

        public Binding(WeaklyTypedBinding binding) : this(binding.BindingMetadata, binding.Expression, binding.CreationMode, binding.OnDeathAction)
        {
        }
    }
}
