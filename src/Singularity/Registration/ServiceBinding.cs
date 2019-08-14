using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Singularity.Collections;
using Singularity.Exceptions;
using Singularity.Expressions;
using Singularity.Graph;

namespace Singularity
{
    [DebuggerDisplay("{Expression?.Type}")]
    internal sealed class ServiceBinding
    {
        public BindingMetadata BindingMetadata { get; }

        /// <summary>
        /// The dependency types
        /// </summary>
        public SinglyLinkedListNode<Type> ServiceTypes { get; }

        public Expression Expression { get; }

        public Lifetime Lifetime { get; }
        public DisposeBehavior NeedsDispose { get; }
        public Action<object>? Finalizer { get; }


        public ReadOnlyExpressionContext? BaseExpression { get; internal set; }
        public Exception? ResolveError { get; internal set; }

        public ArrayList<InstanceFactory> Factories { get; } = new ArrayList<InstanceFactory>();

        public bool TryGetInstanceFactory(Type type, out InstanceFactory factory)
        {
            factory = Factories.Array.FirstOrDefault(x => x.DependencyType == type);
            return factory != null;
        }

        public ServiceBinding(SinglyLinkedListNode<Type> serviceTypes, BindingMetadata bindingMetadata, Expression expression,
            Lifetime lifetime = Lifetime.Transient, Action<object>? finalizer = null,
            DisposeBehavior needsDispose = DisposeBehavior.Default)
        {
            ServiceTypes = serviceTypes ?? throw new ArgumentNullException(nameof(serviceTypes));
            BindingMetadata = bindingMetadata ?? throw new ArgumentNullException(nameof(bindingMetadata));
            Lifetime = !EnumMetadata<Lifetime>.IsValidValue(lifetime) ? throw new InvalidEnumValueException<Lifetime>(lifetime) : lifetime;
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            NeedsDispose = !EnumMetadata<DisposeBehavior>.IsValidValue(needsDispose) ? throw new InvalidEnumValueException<DisposeBehavior>(needsDispose) : needsDispose;
            Finalizer = finalizer;
        }

        public ServiceBinding(Type dependencyType, BindingMetadata bindingMetadata, Expression expression,
            Lifetime lifetime = Lifetime.Transient, Action<object>? finalizer = null,
            DisposeBehavior needsDispose = DisposeBehavior.Default) : this(new SinglyLinkedListNode<Type>(dependencyType), bindingMetadata, expression, lifetime, finalizer, needsDispose)
        {
        }
    }
}
