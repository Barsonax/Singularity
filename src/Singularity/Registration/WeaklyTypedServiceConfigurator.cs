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
        internal WeaklyTypedServiceConfigurator(Type dependencyType, Type instanceType, in BindingMetadata bindingMetadata, SingularitySettings settings, IConstructorResolver? constructorSelector)
        {
            ServiceTypeValidator.CheckIsEnumerable(dependencyType);
            ServiceTypeValidator.CheckIsAssignable(dependencyType, instanceType);
            _instanceType = instanceType;
            _bindingMetadata = bindingMetadata;
            _dependencyTypes = new SinglyLinkedListNode<Type>(dependencyType);
            _settings = settings;
            _constructorSelector = constructorSelector;
        }

        private readonly BindingMetadata _bindingMetadata;
        private readonly SingularitySettings _settings;
        private readonly IConstructorResolver? _constructorSelector;
        private readonly Type _instanceType;
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
                if (_instanceType.ContainsGenericParameters)
                {
                    return new ServiceBinding(_dependencyTypes, _bindingMetadata, null, _instanceType, constructorSelector, _lifetime, _finalizer, _disposeBehavior);
                }
                else
                {
                    _expression = constructorSelector.TryResolveConstructorExpression(_instanceType);
                }
            }
            return new ServiceBinding(_dependencyTypes, _bindingMetadata, _expression, _instanceType, constructorSelector, _lifetime, _finalizer, _disposeBehavior);
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
            ServiceTypeValidator.CheckIsAssignable(_dependencyTypes, expression.GetReturnType());
            _expression = expression;
            return this;
        }

        /// <summary>
        /// Adds a alternative service type.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public WeaklyTypedServiceConfigurator As(Type serviceType)
        {
            ServiceTypeValidator.CheckIsEnumerable(serviceType);
            ServiceTypeValidator.CheckIsAssignable(serviceType, _instanceType);
            _dependencyTypes = new SinglyLinkedListNode<Type>(_dependencyTypes, serviceType);
            return this;
        }
    }
}
