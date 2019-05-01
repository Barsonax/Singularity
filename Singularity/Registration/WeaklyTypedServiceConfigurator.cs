using System;
using System.Linq.Expressions;
using Singularity.Collections;
using Singularity.Exceptions;
using Singularity.Expressions;
using Singularity.Graph;

namespace Singularity
{
    /// <summary>
    /// A weakly typed configurator for registering new bindings.
    /// </summary>
    public sealed class WeaklyTypedServiceConfigurator
    {
        internal WeaklyTypedServiceConfigurator(Type dependencyType, Type instanceType, string callerFilePath, int callerLineNumber, IModule? module = null)
        {
            ServiceTypeValidator.CheckIsEnumerable(dependencyType);
            ServiceTypeValidator.CheckIsAssignable(dependencyType, instanceType);
            _instanceType = instanceType;
            _module = module;
            _callerFilePath = callerFilePath;
            _callerLineNumber = callerLineNumber;
            _dependencyTypes = new SinglyLinkedListNode<Type>(dependencyType);
        }

        private readonly IModule? _module;
        private readonly string _callerFilePath;
        private readonly int _callerLineNumber;
        private readonly Type _instanceType;
        private SinglyLinkedListNode<Type> _dependencyTypes;
        private Expression? _expression;
        private Lifetime _lifetime;
        private Action<object>? _finalizer;
        private DisposeBehavior _disposeBehavior;

        internal Binding ToBinding()
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
            var bindingMetadata = new BindingMetadata(_dependencyTypes, _callerFilePath, _callerLineNumber, _module);
            return new Binding(bindingMetadata, _expression, _lifetime, _finalizer, _disposeBehavior);
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
        /// Controls if and when the instance should be disposed. <see cref="DisposeBehavior"/> for more detailed information.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public WeaklyTypedServiceConfigurator With(DisposeBehavior value)
        {
            if (!EnumMetadata<DisposeBehavior>.IsValidValue(value)) throw new InvalidEnumValue<DisposeBehavior>(value);
            _disposeBehavior = value;
            return this;
        }

        /// <summary>
        /// Controls when should new instances be created. See <see cref="Lifetime"/> for more detailed information.
        /// <param name="value"></param>
        /// </summary>
        public WeaklyTypedServiceConfigurator With(Lifetime value)
        {
            if (!EnumMetadata<Lifetime>.IsValidValue(value)) throw new InvalidEnumValue<Lifetime>(value);
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
