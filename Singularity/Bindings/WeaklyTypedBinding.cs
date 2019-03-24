using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Singularity.Graph;

namespace Singularity.Bindings
{
    /// <summary>
    /// Represents a weakly typed registration
    /// </summary>
    public class WeaklyTypedBinding : IBinding
    {
        /// <summary>
        /// The metadata of this binding.
        /// </summary>
        public BindingMetadata BindingMetadata { get; }

        public Type DependencyType { get; }

        public Expression? Expression { get; }

        public ILifetime Lifetime { get; private set; }

        public Action<object>? OnDeathAction { get; private set; }

        public IReadOnlyList<IDecoratorBinding>? Decorators { get; }

        internal WeaklyTypedBinding(Type dependencyType, Expression? expression, string callerFilePath, int callerLineNumber, IModule? module)
        {
            BindingMetadata = new BindingMetadata(callerFilePath, callerLineNumber, module);
            DependencyType = dependencyType;
            Expression = expression;
            Lifetime = Lifetimes.Transient;
        }

        public WeaklyTypedBinding With(ILifetime lifetime)
        {
            Lifetime = lifetime;
            return this;
        }

        public WeaklyTypedBinding OnDeath(Action<object> onDeathAction)
        {
            OnDeathAction = onDeathAction;
            return this;
        }
    }
}
