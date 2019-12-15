using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Singularity.Bindings;
using Singularity.Collections;
using Singularity.Exceptions;
using Singularity.Lifetime;
using Singularity.Resolvers;

namespace Singularity.Configurators
{
    /// <summary>
    /// A strongly typed configurator for registering new service bindings.
    /// </summary>
    /// <typeparam name="TDependency"></typeparam>
    /// <typeparam name="TInstance"></typeparam>
    public sealed class StronglyTypedServiceConfigurator<TDependency, TInstance>
        where TInstance : class, TDependency
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal StronglyTypedServiceConfigurator(in BindingMetadata bindingMetadata, SingularitySettings settings, IConstructorResolver? constructorSelector)
        {
            ServiceTypeValidator.Cache<TDependency>.CheckIsEnumerable();
            _bindingMetadata = bindingMetadata;
            _dependencyTypes = SinglyLinkedListNodeTypeCache<TDependency>.Instance;
            _settings = settings;
            _constructorSelector = constructorSelector;
        }

        private readonly BindingMetadata _bindingMetadata;
        private readonly SingularitySettings _settings;
        private readonly IConstructorResolver? _constructorSelector;
        private SinglyLinkedListNode<Type> _dependencyTypes;
        private Expression? _expression;
        private ILifetime _lifetime = Lifetimes.Transient;
        private Action<object>? _finalizer;
        private ServiceAutoDispose _disposeBehavior;

        internal ServiceBinding ToBinding()
        {
            var constructorSelector = _constructorSelector ?? _settings.ConstructorResolver;
            if (_expression == null)
            {
                if (TypeMetadataCache<TInstance>.IsInterface) throw new BindingConfigException($"{typeof(TInstance)} cannot be a interface");
                _expression = constructorSelector.ResolveConstructorExpression(typeof(TInstance));
            }
            return new ServiceBinding(_dependencyTypes, _bindingMetadata, _expression, typeof(TInstance), constructorSelector, _lifetime, _finalizer, _disposeBehavior);
        }

        /// <summary>
        /// A action that is executed when the <see cref="Scoped"/> or <see cref="Container"/> is disposed.
        /// <param name="onDeathAction"></param>.
        /// </summary>
        public StronglyTypedServiceConfigurator<TDependency, TInstance> WithFinalizer(Action<TInstance> onDeathAction)
        {
            _finalizer = obj => onDeathAction((TInstance)obj);
            return this;
        }

        /// <summary>
        /// Controls if and when the instance should be disposed. <see cref="ServiceAutoDispose"/> for more detailed information.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StronglyTypedServiceConfigurator<TDependency, TInstance> With(ServiceAutoDispose value)
        {
            _disposeBehavior = value;
            return this;
        }

        /// <summary>
        /// Controls when should new instances be created. See <see cref="Lifetimes"/> for more detailed information.
        /// <param name="value"></param>
        /// </summary>
        public StronglyTypedServiceConfigurator<TDependency, TInstance> With(ILifetime value)
        {
            _lifetime = value;
            return this;
        }

        /// <summary>
        /// Adds a alternative service type.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public StronglyTypedServiceConfigurator<TDependency, TInstance> As<TService>()
        {
            _dependencyTypes = new SinglyLinkedListNode<Type>(_dependencyTypes, typeof(TService));
            return this;
        }

        /// <summary>
        /// Lets you provide a expression <see cref="System.Linq.Expressions.Expression"/> to create the instance constructor.
        /// Be careful as there will be no exception if this expression returns a null instance.
        /// </summary>
        /// <returns></returns>
        public StronglyTypedServiceConfigurator<TDependency, TInstance> Inject(Expression<Func<TInstance>> expression) => InjectInternal(expression);

        /// <summary>
        /// <see cref="Inject(Expression{Func{TInstance}})"/>
        /// </summary>
		public StronglyTypedServiceConfigurator<TDependency, TInstance> Inject<TP1>(Expression<Func<TP1, TInstance>> expression) => InjectInternal(expression);

        /// <summary>
        /// <see cref="Inject(Expression{Func{TInstance}})"/>
        /// </summary>
        public StronglyTypedServiceConfigurator<TDependency, TInstance> Inject<TP1, TP2>(Expression<Func<TP1, TP2, TInstance>> expression) => InjectInternal(expression);

        /// <summary>
        /// <see cref="Inject(Expression{Func{TInstance}})"/>
        /// </summary>
        public StronglyTypedServiceConfigurator<TDependency, TInstance> Inject<TP1, TP2, TP3>(Expression<Func<TP1, TP2, TP3, TInstance>> expression) => InjectInternal(expression);

        /// <summary>
        /// <see cref="Inject(Expression{Func{TInstance}})"/>
        /// </summary>
        public StronglyTypedServiceConfigurator<TDependency, TInstance> Inject<TP1, TP2, TP3, TP4>(Expression<Func<TP1, TP2, TP3, TP4, TInstance>> expression) => InjectInternal(expression);

        /// <summary>
        /// <see cref="Inject(Expression{Func{TInstance}})"/>
        /// </summary>
        public StronglyTypedServiceConfigurator<TDependency, TInstance> Inject<TP1, TP2, TP3, TP4, TP5>(Expression<Func<TP1, TP2, TP3, TP4, TP5, TInstance>> expression) => InjectInternal(expression);

        /// <summary>
        /// <see cref="Inject(Expression{Func{TInstance}})"/>
        /// </summary>
        public StronglyTypedServiceConfigurator<TDependency, TInstance> Inject<TP1, TP2, TP3, TP4, TP5, TP6>(Expression<Func<TP1, TP2, TP3, TP4, TP5, TP6, TInstance>> expression) => InjectInternal(expression);

        /// <summary>
        /// <see cref="Inject(Expression{Func{TInstance}})"/>
        /// </summary>
        public StronglyTypedServiceConfigurator<TDependency, TInstance> Inject<TP1, TP2, TP3, TP4, TP5, TP6, TP7>(Expression<Func<TP1, TP2, TP3, TP4, TP5, TP6, TP7, TInstance>> expression) => InjectInternal(expression);

        /// <summary>
        /// <see cref="Inject(Expression{Func{TInstance}})"/>
        /// </summary>
        public StronglyTypedServiceConfigurator<TDependency, TInstance> Inject<TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8>(Expression<Func<TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TInstance>> expression) => InjectInternal(expression);

        private StronglyTypedServiceConfigurator<TDependency, TInstance> InjectInternal(Expression expression)
        {
            _expression = expression;
            return this;
        }
    }
}
