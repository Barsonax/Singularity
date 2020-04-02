using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using Singularity.Collections;
using Singularity.Exceptions;
using Singularity.Expressions;

namespace Singularity
{
    /// <summary>
    /// A strongly typed configurator for registering new service bindings.
    /// </summary>
    /// <typeparam name="TImplementation"></typeparam>
    public sealed class StronglyTypedServiceConfigurator<TImplementation>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal StronglyTypedServiceConfigurator(in BindingMetadata bindingMetadata, SinglyLinkedListNode<Type> serviceTypes, SingularitySettings settings)
        {
            _bindingMetadata = bindingMetadata;
            _serviceTypes = serviceTypes;
            _settings = settings;
        }

        private readonly BindingMetadata _bindingMetadata;
        private readonly SingularitySettings _settings;
        private readonly SinglyLinkedListNode<Type> _serviceTypes;
        private IConstructorResolver? _constructorSelector;        
        private Expression? _expression;
        private ILifetime _lifetime = Lifetimes.Transient;
        private Action<object>? _finalizer;
        private ServiceAutoDispose _disposeBehavior;

        internal ServiceBinding ToBinding()
        {
            var constructorSelector = _constructorSelector ?? _settings.ConstructorResolver;
            if (_expression == null)
            {
                if (TypeMetadataCache<TImplementation>.IsInterface) throw new BindingConfigException($"{typeof(TImplementation)} cannot be a interface");
                _expression = constructorSelector.TryResolveConstructorExpression(typeof(TImplementation));
            }
            return new ServiceBinding(_serviceTypes, _bindingMetadata, _expression, typeof(TImplementation), constructorSelector, _lifetime, _finalizer, _disposeBehavior);
        }

        /// <summary>
        /// A action that is executed when the <see cref="Scoped"/> or <see cref="Container"/> is disposed.
        /// <param name="onDeathAction"></param>.
        /// </summary>
        public StronglyTypedServiceConfigurator<TImplementation> WithFinalizer(Action<TImplementation> onDeathAction)
        {
            _finalizer = obj => onDeathAction((TImplementation)obj);
            return this;
        }

        /// <summary>
        /// Controls if and when the instance should be disposed. <see cref="ServiceAutoDispose"/> for more detailed information.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StronglyTypedServiceConfigurator<TImplementation> With(ServiceAutoDispose value)
        {
            _disposeBehavior = value;
            return this;
        }

        /// <summary>
        /// Controls when should new instances be created. See <see cref="Lifetimes"/> for more detailed information.
        /// <param name="value"></param>
        /// </summary>
        public StronglyTypedServiceConfigurator<TImplementation> With(ILifetime value)
        {
            _lifetime = value;
            return this;
        }

        /// <summary>
        /// Overrides the default constructor selector for this dependency
        /// <param name="value"></param>
        /// </summary>
        public StronglyTypedServiceConfigurator<TImplementation> With(IConstructorResolver value)
        {
            _constructorSelector = value;
            return this;
        }

        /// <summary>
        /// Lets you provide a expression <see cref="System.Linq.Expressions.Expression"/> to create the instance constructor.
        /// Be careful as there will be no exception if this expression returns a null instance.
        /// </summary>
        /// <returns></returns>
        public StronglyTypedServiceConfigurator<TImplementation> Inject(Expression<Func<TImplementation>> expression) => InjectInternal(expression);

        /// <summary>
        /// <see cref="Inject(Expression{Func{TImplementation}})"/>
        /// </summary>
		public StronglyTypedServiceConfigurator<TImplementation> Inject<TP1>(Expression<Func<TP1, TImplementation>> expression) => InjectInternal(expression);

        /// <summary>
        /// <see cref="Inject(Expression{Func{TImplementation}})"/>
        /// </summary>
        public StronglyTypedServiceConfigurator<TImplementation> Inject<TP1, TP2>(Expression<Func<TP1, TP2, TImplementation>> expression) => InjectInternal(expression);

        /// <summary>
        /// <see cref="Inject(Expression{Func{TImplementation}})"/>
        /// </summary>
        public StronglyTypedServiceConfigurator<TImplementation> Inject<TP1, TP2, TP3>(Expression<Func<TP1, TP2, TP3, TImplementation>> expression) => InjectInternal(expression);

        /// <summary>
        /// <see cref="Inject(Expression{Func{TImplementation}})"/>
        /// </summary>
        public StronglyTypedServiceConfigurator<TImplementation> Inject<TP1, TP2, TP3, TP4>(Expression<Func<TP1, TP2, TP3, TP4, TImplementation>> expression) => InjectInternal(expression);

        /// <summary>
        /// <see cref="Inject(Expression{Func{TImplementation}})"/>
        /// </summary>
        public StronglyTypedServiceConfigurator<TImplementation> Inject<TP1, TP2, TP3, TP4, TP5>(Expression<Func<TP1, TP2, TP3, TP4, TP5, TImplementation>> expression) => InjectInternal(expression);

        /// <summary>
        /// <see cref="Inject(Expression{Func{TImplementation}})"/>
        /// </summary>
        public StronglyTypedServiceConfigurator<TImplementation> Inject<TP1, TP2, TP3, TP4, TP5, TP6>(Expression<Func<TP1, TP2, TP3, TP4, TP5, TP6, TImplementation>> expression) => InjectInternal(expression);

        /// <summary>
        /// <see cref="Inject(Expression{Func{TImplementation}})"/>
        /// </summary>
        public StronglyTypedServiceConfigurator<TImplementation> Inject<TP1, TP2, TP3, TP4, TP5, TP6, TP7>(Expression<Func<TP1, TP2, TP3, TP4, TP5, TP6, TP7, TImplementation>> expression) => InjectInternal(expression);

        /// <summary>
        /// <see cref="Inject(Expression{Func{TImplementation}})"/>
        /// </summary>
        public StronglyTypedServiceConfigurator<TImplementation> Inject<TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8>(Expression<Func<TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TImplementation>> expression) => InjectInternal(expression);

        private StronglyTypedServiceConfigurator<TImplementation> InjectInternal(Expression expression)
        {
            _expression = expression;
            return this;
        }
    }
}
