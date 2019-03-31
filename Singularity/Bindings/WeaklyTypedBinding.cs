using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

        /// <summary>
        /// The type of the dependency this binding is defined for, usually a interface.
        /// </summary>
        public Type DependencyType { get; }

        /// <summary>
        /// A expression that is used to create the instance
        /// </summary>
        public Expression? Expression => WeaklyTypedConfiguredBinding?.Expression;

        /// <summary>
        /// When should new instance be created. See <see cref="CreationMode"/> for more detailed information.
        /// </summary>
        public CreationMode CreationMode => WeaklyTypedConfiguredBinding?.CreationMode ?? CreationMode.Transient;

        /// <summary>
        /// A action that is executed when the <see cref="Scoped"/> is disposed. This usually happens when the <see cref="Container"/> is disposed.
        /// </summary>
        public Action<object>? OnDeathAction => WeaklyTypedConfiguredBinding?.OnDeathAction;

        private protected WeaklyTypedConfiguredBinding? WeaklyTypedConfiguredBinding { get; set; }

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

        /// <summary>
        /// Sets the expression that is used to create the instance(s)
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
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

        internal void Verify()
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
