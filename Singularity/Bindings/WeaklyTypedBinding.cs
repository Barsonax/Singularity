using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Exceptions;
using Singularity.Graph;

namespace Singularity.Bindings
{
    /// <summary>
    /// Represents a weakly typed registration
    /// </summary>
    public class WeaklyTypedBinding
    {
        /// <summary>
        /// The metadata of this binding.
        /// </summary>
        public BindingMetadata BindingMetadata { get; }

        public Type DependencyType { get; }

        public WeaklyTypedConfiguredBinding? WeaklyTypedConfiguredBinding { get; protected set; }

        public Expression? Expression => WeaklyTypedConfiguredBinding?.Expression;

        public CreationMode CreationMode => WeaklyTypedConfiguredBinding?.CreationMode ?? CreationMode.Transient;

        public Action<object>? OnDeathAction => WeaklyTypedConfiguredBinding?.OnDeathAction;

        /// <summary>
        /// The decorators for this binding.
        /// </summary>
        public List<WeaklyTypedDecoratorBinding>? Decorators { get; private set; }

        internal bool Locked { get; private set; }

        internal WeaklyTypedBinding(Type dependencyType, string callerFilePath, int callerLineNumber, IModule? module)
        {
            DependencyType = dependencyType ?? throw new ArgumentNullException(nameof(dependencyType));
            BindingMetadata = new BindingMetadata(dependencyType ,callerFilePath, callerLineNumber, module);
        }

        public WeaklyTypedBinding(Type dependencyType, BindingMetadata bindingMetadata)
        {
            DependencyType = dependencyType ?? throw new ArgumentNullException(nameof(dependencyType));
            BindingMetadata = bindingMetadata ?? throw new ArgumentNullException(nameof(bindingMetadata));
        }

        public WeaklyTypedConfiguredBinding Inject(Expression expression)
        {
            WeaklyTypedConfiguredBinding = new WeaklyTypedConfiguredBinding(this, expression);
            return WeaklyTypedConfiguredBinding;
        }

        internal void AddDecorator(WeaklyTypedDecoratorBinding weaklyTypedDecoratorBinding)
        {
            if (Decorators == null) Decorators = new List<WeaklyTypedDecoratorBinding>();
            Decorators.Add(weaklyTypedDecoratorBinding);
        }

        public void Verify()
        {
            if (Expression == null && Decorators == null)
                throw new BindingConfigException($"The binding at {BindingMetadata.StringRepresentation()} does not have a expression");
            if (Decorators != null)
            {
                foreach (WeaklyTypedDecoratorBinding weaklyTypedDecoratorBinding in Decorators)
                {
                    if (weaklyTypedDecoratorBinding.Expression == null)
                        throw new BindingConfigException($"The decorator for {DependencyType} does not have a expression");
                }
            }

            Locked = true;
        }
    }
}
