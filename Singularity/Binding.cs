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
    internal readonly struct Binding
    {
        public BindingMetadata BindingMetadata { get; }
        public Type DependencyType { get; }
        public Expression? Expression { get; }
        public ReadOnlyCollection<ParameterExpression> Parameters { get; }
        public ReadOnlyCollection<ParameterExpression> DecoratorParameters { get; }

        public CreationMode CreationMode { get; }
        public Action<object>? OnDeathAction { get; }
        public Expression[] Decorators { get; }

        private static readonly ReadOnlyCollection<ParameterExpression> EmptyParameters = new ReadOnlyCollection<ParameterExpression>(new ParameterExpression[0]);

        public Binding(BindingMetadata bindingMetadata, Type dependencyType, Expression? expression, CreationMode creationMode, Expression[] decorators, Action<object>? onDeath)
        {
            BindingMetadata = bindingMetadata ?? throw new ArgumentNullException(nameof(bindingMetadata));
            DependencyType = dependencyType ?? throw new ArgumentNullException(nameof(dependencyType));
            CreationMode = creationMode;

            Expression = expression;
            Parameters = expression != null ? new ReadOnlyCollection<ParameterExpression>(expression.GetParameterExpressions()) : EmptyParameters;

            Decorators = decorators ?? throw new ArgumentNullException(nameof(decorators));
            DecoratorParameters = decorators.Length != 0
                ? new ReadOnlyCollection<ParameterExpression>(
                    Decorators.SelectMany(x => x.GetParameterExpressions())
                              .Where(x => x.Type != dependencyType).ToArray())
                : EmptyParameters;
            OnDeathAction = onDeath;
        }

        public Binding(WeaklyTypedBinding binding) : this(binding.BindingMetadata, binding.DependencyType, binding.Expression, binding.CreationMode,
            binding.Decorators != null ? binding.Decorators.Select(x => x.Expression!).ToArray() : new Expression[0], binding.OnDeathAction)
        {
        }
    }
}
