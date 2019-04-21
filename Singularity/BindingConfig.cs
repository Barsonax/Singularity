using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Singularity.Bindings;
using Singularity.Collections;
using Singularity.Exceptions;
using Singularity.Expressions;

namespace Singularity
{
    /// <summary>
    /// A class to make configuring dependencies easier
    /// </summary>
    public sealed class BindingConfig
    {
        internal IModule? CurrentModule { set => _registrations.CurrentModule = value; }
        private readonly RegistrationStore _registrations = new RegistrationStore();

        /// <summary>
        /// Begins configuring a strongly typed binding for <typeparamref name="TDependency"/>
        /// </summary>
        /// <typeparam name="TDependency"></typeparam>
        /// <returns></returns>
        public StronglyTypedBinding<TDependency> Register<TDependency>([CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            return new StronglyTypedBinding<TDependency>(_registrations.CreateBinding(typeof(TDependency), callerFilePath, callerLineNumber));
        }

        /// <summary>
        /// Registers a new strongly typed dependency and auto resolves a expression to create it.
        /// </summary>
        /// <typeparam name="TDependency"></typeparam>
        /// <typeparam name="TInstance"></typeparam>
        /// <returns></returns>
        public StronglyTypedConfiguredBinding<TDependency, TInstance> Register<TDependency, TInstance>([CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
            where TInstance : class, TDependency
        {
            StronglyTypedBinding<TDependency> binding = Register<TDependency>(callerFilePath, callerLineNumber);
            return binding.Inject<TInstance>(AutoResolveConstructorExpressionCache<TInstance>.Expression);
        }

        /// <summary>
        /// Begins configuring a weakly typed binding for <paramref name="dependencyType"/>
        /// </summary>
        /// <param name="dependencyType"></param>
        /// <returns></returns>
#pragma warning disable 1573
        public WeaklyTypedBinding Register(Type dependencyType, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
#pragma warning restore 1573
        {
            return _registrations.CreateBinding(dependencyType, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a new weakly typed dependency and auto resolves a expression to create it.
        /// </summary>
        /// <param name="dependencyType"></param>
        /// <param name="instanceType"></param>
        /// <returns></returns>
#pragma warning disable 1573
        public WeaklyTypedConfiguredBinding Register(Type dependencyType, Type instanceType, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
#pragma warning restore 1573
        {
            if (dependencyType.ContainsGenericParameters)
            {

            }
            else
            {
                if (!dependencyType.IsAssignableFrom(instanceType)) throw new TypeNotAssignableException($"{dependencyType} is not implemented by {instanceType}");
            }

            WeaklyTypedBinding binding = Register(dependencyType, callerFilePath, callerLineNumber);

            Expression expression;
            if (dependencyType.ContainsGenericParameters && instanceType.ContainsGenericParameters)
            {
                expression = new OpenGenericTypeExpression(instanceType);
            }
            else
            {
                expression = instanceType.AutoResolveConstructorExpression();
            }
            return binding.Inject(expression);
        }

        /// <summary>
        /// Registers a batch of new weakly typed dependencies and auto resolves expressions to create them.
        /// </summary>
        /// <param name="dependencyType"></param>
        /// <param name="instanceTypes"></param>
        /// <returns></returns>
#pragma warning disable 1573
        public WeaklyTypedBindingBatch Register(Type dependencyType, Type[] instanceTypes, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
#pragma warning restore 1573
        {
            var bindings = new WeaklyTypedConfiguredBinding[instanceTypes.Length];
            for (var index = 0; index < instanceTypes.Length; index++)
            {
                Type instanceType = instanceTypes[index];
                bindings[index] = Register(dependencyType, instanceType, callerFilePath, callerLineNumber);
            }

            return new WeaklyTypedBindingBatch(new ReadOnlyCollection<WeaklyTypedConfiguredBinding>(bindings));
        }

        /// <summary>
        /// Registers a new strongly typed decorator for <typeparamref name="TDependency"/>.
        /// </summary>
        /// <typeparam name="TDependency">The type to decorate</typeparam>
        /// <typeparam name="TDecorator"></typeparam>
        /// <exception cref="InterfaceExpectedException">If <typeparamref name="TDependency"/> is not a interface</exception>
        /// <returns></returns>
        public StronglyTypedDecoratorBinding<TDependency> Decorate<TDependency, TDecorator>([CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
            where TDependency : class
            where TDecorator : TDependency
        {
            return new StronglyTypedDecoratorBinding<TDependency>(_registrations.CreateDecorator(typeof(TDependency), typeof(TDecorator)));
        }

        /// <summary>
        /// Registers a new weakly typed decorator for <paramref name="dependencyType"/>
        /// </summary>
        /// <param name="dependencyType"></param>
        /// <param name="decoratorType"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        public WeaklyTypedDecoratorBinding Decorate(Type dependencyType, Type decoratorType, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            return _registrations.CreateDecorator(dependencyType, decoratorType);
        }

        /// <summary>
        /// Registers a batch of new weakly typed decorators for <paramref name="dependencyType"/> and auto resolves expressions to create them.
        /// </summary>
        /// <param name="dependencyType"></param>
        /// <param name="decoratorTypes"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        public WeaklyTypedDecoratorBindingBatch Decorate(Type dependencyType, Type[] decoratorTypes, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            var bindings = new WeaklyTypedDecoratorBinding[decoratorTypes.Length];
            for (var index = 0; index < decoratorTypes.Length; index++)
            {
                Type decoratorType = decoratorTypes[index];
                bindings[index] = Decorate(dependencyType, decoratorType);
            }

            return new WeaklyTypedDecoratorBindingBatch(new ReadOnlyCollection<WeaklyTypedDecoratorBinding>(bindings));
        }

        internal ReadOnlyCollection<ReadonlyRegistration> GetDependencies() => _registrations.GetDependencies();
    }
}