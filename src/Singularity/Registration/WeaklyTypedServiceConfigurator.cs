using System;
using System.Linq.Expressions;
using Singularity.Collections;
using Singularity.Expressions;
using Singularity.Graph;

namespace Singularity
{
    /// <summary>
    /// A weakly typed configurator for registering new bindings.
    /// </summary>
    public sealed class WeaklyTypedServiceConfigurator
    {
        internal WeaklyTypedServiceConfigurator(Type dependencyType, Type instanceType, BindingMetadata bindingMetadata)
        {
            ServiceTypeValidator.CheckIsEnumerable(dependencyType);
            ServiceTypeValidator.CheckIsAssignable(dependencyType, instanceType);
            _instanceType = instanceType;
            _bindingMetadata = bindingMetadata;
            _dependencyTypes = new SinglyLinkedListNode<Type>(dependencyType);
        }

        private readonly BindingMetadata _bindingMetadata;
        private readonly Type _instanceType;
        private SinglyLinkedListNode<Type> _dependencyTypes;
        private Expression? _expression;
        private ILifetime _lifetime;
        private Action<object>? _finalizer;
        private ServiceAutoDispose _disposeBehavior;

        internal ServiceBinding ToBinding()
        {
            if (_expression == null)
            {
                if (_instanceType.ContainsGenericParameters)
                {
                    _expression = new AbstractBindingExpression(_instanceType);
                }
                else
                {
                    _expression = _instanceType.AutoResolveConstructorExpression();
                }
            }
            return new ServiceBinding(_dependencyTypes, _bindingMetadata, _expression, _lifetime, _finalizer, _disposeBehavior);
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
            _dependencyTypes = new SinglyLinkedListNode<Type>(_dependencyTypes, serviceType);
            return this;
        }
    }
}
