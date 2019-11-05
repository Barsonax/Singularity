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
    /// <summary>
    /// Contains information that is needed to properly resolve a service.
    /// </summary>
    [DebuggerDisplay("{Expression?.Type}")]
    public sealed class ServiceBinding
    {
        /// <summary>
        /// The metadata such as where this binding originates from.
        /// </summary>
        public BindingMetadata BindingMetadata { get; }

        /// <summary>
        /// The dependency types
        /// </summary>
        public SinglyLinkedListNode<Type> ServiceTypes { get; }

        /// <summary>
        /// A expression that creates the service but all its dependencies are not yet resolved.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// The lifetime of the service.
        /// </summary>
        public ILifetime Lifetime { get; }

        /// <summary>
        /// Denotes if this service should be automatically disposed. <see cref="ServiceAutoDispose"/> for more info.
        /// </summary>
        public ServiceAutoDispose NeedsDispose { get; }

        /// <summary>
        /// The finalizer of the service that will be called after its disposed.
        /// Not to be confused with the finalizer of a type itself.
        /// </summary>
        public Action<object>? Finalizer { get; }

        /// <summary>
        /// The constructor resolver that is used to resolve the service's constructor.
        /// </summary>
        public IConstructorResolver ConstructorResolver { get; }

        /// <summary>
        /// The base expression of to create a instance of this service. Dependencies are resolved and the lifetime has been applied.
        /// However decorators have not been applied.
        /// </summary>
        public ReadOnlyExpressionContext? BaseExpression { get; internal set; }

        /// <summary>
        /// Will store the error if a error occurs when resolving the service.
        /// </summary>
        public Exception? ResolveError { get; internal set; }

        /// <summary>
        /// A list of factories for different service types.
        /// These factories are generated using the <see cref="BaseExpression"/> as input.
        /// Decorators are applied at this point.
        /// </summary>
        public ArrayList<InstanceFactory> Factories { get; } = new ArrayList<InstanceFactory>();

        /// <summary>
        /// Gets the instance factory if it exists for the given type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public bool TryGetInstanceFactory(Type type, out InstanceFactory factory)
        {
            factory = Factories.Array.FirstOrDefault(x => x.DependencyType == type);
            return factory != null;
        }

        public ServiceBinding(SinglyLinkedListNode<Type> serviceTypes, in BindingMetadata bindingMetadata, Expression expression, IConstructorResolver constructorResolver,
            ILifetime lifetime, Action<object>? finalizer = null,
            ServiceAutoDispose needsDispose = ServiceAutoDispose.Default)
        {
            ServiceTypes = serviceTypes ?? throw new ArgumentNullException(nameof(serviceTypes));
            BindingMetadata = bindingMetadata;
            Lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            NeedsDispose = !EnumMetadata<ServiceAutoDispose>.IsValidValue(needsDispose) ? throw new InvalidEnumValueException<ServiceAutoDispose>(needsDispose) : needsDispose;
            Finalizer = finalizer;
            ConstructorResolver = constructorResolver;
        }

        public ServiceBinding(Type dependencyType, in BindingMetadata bindingMetadata, Expression expression, IConstructorResolver constructorResolver,
            ILifetime? lifetime = null, Action<object>? finalizer = null,
            ServiceAutoDispose needsDispose = ServiceAutoDispose.Default) : this(new SinglyLinkedListNode<Type>(dependencyType), bindingMetadata, expression, constructorResolver, lifetime ?? Lifetimes.Transient, finalizer, needsDispose)
        {
        }
    }
}
