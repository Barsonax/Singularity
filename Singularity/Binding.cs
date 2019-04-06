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
        public Type DependencyType { get; }
        public Expression? Expression { get; }
        public ReadOnlyCollection<ParameterExpression> Parameters { get; }

        public CreationMode CreationMode { get; }
        public Action<object>? OnDeathAction { get; }

        private static readonly ReadOnlyCollection<ParameterExpression> EmptyParameters = new ReadOnlyCollection<ParameterExpression>(new ParameterExpression[0]);

        public Binding(BindingMetadata bindingMetadata, Type dependencyType, Expression? expression, CreationMode creationMode, Action<object>? onDeath)
        {
            BindingMetadata = bindingMetadata ?? throw new ArgumentNullException(nameof(bindingMetadata));
            DependencyType = dependencyType ?? throw new ArgumentNullException(nameof(dependencyType));
            CreationMode = creationMode;

            Expression = expression;
            Parameters = expression != null ? new ReadOnlyCollection<ParameterExpression>(expression.GetParameterExpressions()) : EmptyParameters;

            OnDeathAction = onDeath;
        }

        public Binding(WeaklyTypedBinding binding) : this(binding.BindingMetadata, binding.DependencyType, binding.Expression, binding.CreationMode, binding.OnDeathAction)
        {
        }
    }
}
