using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Singularity.Graph
{
    public sealed class UnresolvedDependency
    {
        public BindingMetadata BindingMetadata => _binding.BindingMetadata;
        public Type DependencyType => _binding.DependencyType;
        public Expression Expression => _binding.Expression!;
        public ILifetime Lifetime => _binding.Lifetime;
        public Action<object>? OnDeath => _binding.OnDeath;
        public IReadOnlyCollection<DecoratorBinding> Decorators => _binding.Decorators;

        private readonly Binding _binding;

        public UnresolvedDependency(Binding binding)
        {
            if (binding.Expression == null) throw new ArgumentNullException(nameof(binding.Expression));
            _binding = binding;
        }
    }
}