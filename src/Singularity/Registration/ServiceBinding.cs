using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Singularity.Collections;
using Singularity.Exceptions;
using Singularity.Expressions;
using Singularity.Graph;
using Singularity.Graph.Resolvers;

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
        /// A expression that creates the service instance but all its dependencies are not yet resolved.
        /// </summary>
        public Expression? Expression { get; internal set; }

        /// <summary>
        /// The concrete type of this service.
        /// </summary>
        public Type ConcreteType { get; }

        /// <summary>
        /// The lifetime of the service.
        /// </summary>
        public ILifetime Lifetime { get; }

        /// <summary>
        /// Denotes if the service instance should be automatically disposed. See <see cref="ServiceAutoDispose"/> for more info.
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
        /// The base expression of to create a instance of this service. Dependencies are resolved and the lifetime has been applied, however decorators have not yet been applied.
        /// Do not use this directly but use <see cref="Factories"/> instead.
        /// </summary>
        public ReadOnlyExpressionContext? BaseExpression { get; internal set; }

        /// <summary>
        /// Will store the error if a error occurs when resolving the service.
        /// </summary>
        public Exception? ResolveError { get; internal set; }

        /// <summary>
        /// A list of factories for different service types.
        /// These factories are generated using the <see cref="BaseExpression"/> as input.
        /// Decorators are applied at this point and its safe to use this to resolve dependencies of other services with <see cref="TryGetInstanceFactory"/>
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

        /// <summary>
        /// Creates a new service binding using the provided data.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidEnumValueException{T}"></exception>
        public ServiceBinding(SinglyLinkedListNode<Type> serviceTypes, in BindingMetadata bindingMetadata, Expression? expression, Type concreteType, IConstructorResolver constructorResolver,
            ILifetime lifetime, Action<object>? finalizer = null,
            ServiceAutoDispose needsDispose = ServiceAutoDispose.Default)
        {
            ServiceTypes = serviceTypes ?? throw new ArgumentNullException(nameof(serviceTypes));
            BindingMetadata = bindingMetadata;
            Lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
            Expression = expression;
            ConcreteType = concreteType ?? throw new ArgumentNullException(nameof(concreteType));
            NeedsDispose = !EnumMetadata<ServiceAutoDispose>.IsValidValue(needsDispose) ? throw new InvalidEnumValueException<ServiceAutoDispose>(needsDispose) : needsDispose;
            Finalizer = finalizer;
            ConstructorResolver = constructorResolver;
        }

        /// <summary>
        /// Constructor that fills in some default values to make it more easier to use in <see cref="IServiceBindingGenerator"/>'s
        /// </summary>
        public ServiceBinding(Type dependencyType, in BindingMetadata bindingMetadata, Expression? expression, Type concreteType, IConstructorResolver constructorResolver,
            ILifetime? lifetime = null, Action<object>? finalizer = null,
            ServiceAutoDispose needsDispose = ServiceAutoDispose.Default) : this(new SinglyLinkedListNode<Type>(dependencyType), bindingMetadata, expression, concreteType, constructorResolver, lifetime ?? Lifetimes.Transient, finalizer, needsDispose)
        {
        }
    }
}
