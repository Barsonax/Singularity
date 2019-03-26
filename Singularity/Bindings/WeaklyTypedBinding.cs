using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Singularity.Graph;

namespace Singularity.Bindings
{
    /// <summary>
    /// Represents a weakly typed registration
    /// </summary>
    public abstract class WeaklyTypedBinding
    {
        /// <summary>
        /// The metadata of this binding.
        /// </summary>
        public BindingMetadata BindingMetadata { get; }

        public Type DependencyType { get; }

        public WeaklyTypedConfiguredBinding? WeaklyTypedConfiguredBinding { get; protected set; }

        public Expression? Expression => WeaklyTypedConfiguredBinding?.Expression;

        public ILifetime Lifetime => WeaklyTypedConfiguredBinding?.Lifetime ?? Lifetimes.Transient;

        public Action<object>? OnDeathAction => WeaklyTypedConfiguredBinding?.OnDeathAction;

        /// <summary>
        /// The decorators for this binding.
        /// </summary>
        public List<WeaklyTypedDecoratorBinding>? Decorators { get; internal set; }

        internal WeaklyTypedBinding(Type dependencyType, string callerFilePath, int callerLineNumber, IModule? module)
        {
            DependencyType = dependencyType;
            BindingMetadata = new BindingMetadata(callerFilePath, callerLineNumber, module);
        }

        public abstract WeaklyTypedConfiguredBinding Inject(Expression expression);
    }
}
