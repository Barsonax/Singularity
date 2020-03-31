using System;
using System.Linq.Expressions;

using Singularity.Collections;
using Singularity.Expressions;

namespace Singularity
{
    /// <summary>
    /// A weakly typed configurator for registering new bindings.
    /// </summary>
    public sealed class WeaklyTypedServiceConfigurator
    {
        internal WeaklyTypedServiceConfigurator(SinglyLinkedListNode<Type> serviceTypes, Type implementationType, in BindingMetadata bindingMetadata, SingularitySettings settings)
        {
            foreach (var serviceType in serviceTypes)
            {
                ServiceTypeValidator.CheckIsEnumerable(serviceType);
                ServiceTypeValidator.CheckIsAssignable(serviceType, implementationType);
            }

            _implementationType = implementationType;
            _bindingMetadata = bindingMetadata;
            _serviceTypes = serviceTypes;
            _settings = settings;
        }

        private readonly BindingMetadata _bindingMetadata;
        private readonly SingularitySettings _settings;
        private IConstructorResolver? _constructorSelector;
        private readonly Type _implementationType;
        private SinglyLinkedListNode<Type> _serviceTypes;
        private Expression? _expression;
        private ILifetime _lifetime = Lifetimes.Transient;
        private Action<object>? _finalizer;
        private ServiceAutoDispose _disposeBehavior;


        internal ServiceBinding ToBinding()
        {
            var constructorSelector = _constructorSelector ?? _settings.ConstructorResolver;
            if (_expression == null)
            {
                if (_implementationType.ContainsGenericParameters)
                {
                    return new ServiceBinding(_serviceTypes, _bindingMetadata, null, _implementationType, constructorSelector, _lifetime, _finalizer, _disposeBehavior);
                }
                else
                {
                    _expression = constructorSelector.TryResolveConstructorExpression(_implementationType);
                }
            }
            return new ServiceBinding(_serviceTypes, _bindingMetadata, _expression, _implementationType, constructorSelector, _lifetime, _finalizer, _disposeBehavior);
        }

        /// <summary>
        /// A action that is executed when the <see cref="Scoped"/> or <see cref="Container"/> is disposed.
        /// <param name="onDeathAction"></param>.
        /// </summary>
        public WeaklyTypedServiceConfigurator WithFinalizer(Action<object> onDeathAction)
        {
            _finalizer = onDeathAction;
            return this;
        }

        /// <summary>
        /// Controls if and when the instance should be disposed. <see cref="ServiceAutoDispose"/> for more detailed information.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public WeaklyTypedServiceConfigurator With(ServiceAutoDispose value)
        {
            _disposeBehavior = value;
            return this;
        }

        /// <summary>
        /// Controls when should new instances be created. See <see cref="Lifetimes"/> for more detailed information.
        /// <param name="value"></param>
        /// </summary>
        public WeaklyTypedServiceConfigurator With(ILifetime value)
        {
            _lifetime = value;
            return this;
        }

        /// <summary>
        /// Sets the expression that is used to create the instance(s)
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public WeaklyTypedServiceConfigurator Inject(Expression expression)
        {
            ServiceTypeValidator.CheckIsAssignable(_serviceTypes, expression.GetReturnType());
            _expression = expression;
            return this;
        }

        /// <summary>
        /// Overrides the default constructor selector for this dependency
        /// <param name="value"></param>
        /// </summary>
        public WeaklyTypedServiceConfigurator With(IConstructorResolver value)
        {
            _constructorSelector = value;
            return this;
        }
    }
}
