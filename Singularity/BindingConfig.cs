using System;
using System.Collections.ObjectModel;
using System.Data;
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
            return new StronglyTypedBinding<TDependency>(_registrations.CreateBinding(new[] { typeof(TDependency) }, callerFilePath, callerLineNumber));
        }

        /// <summary>
        /// Registers a new strongly typed dependency and auto resolves a expression to create it.
        /// </summary>
        /// <typeparam name="TDependency1"></typeparam>
        /// <typeparam name="TInstance"></typeparam>
        /// <returns></returns>
        public StronglyTypedConfiguredBinding<TDependency1, TInstance> Register<TDependency1, TInstance>([CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
            where TInstance : class, TDependency1
        {
            var binding = new StronglyTypedBinding<TDependency1>(Register(typeof(TDependency1), callerFilePath, callerLineNumber));
            return binding.Inject<TInstance>(AutoResolveConstructorExpressionCache<TInstance>.Expression);
        }

        public StronglyTypedConfiguredBinding<TDependency1, TInstance> Register<TDependency1, TDependency2, TInstance>([CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
            where TInstance : class, TDependency1, TDependency2
        {
            var binding = new StronglyTypedBinding<TDependency1>(Register(new[] { typeof(TDependency1), typeof(TDependency2) }, callerFilePath, callerLineNumber));
            return binding.Inject<TInstance>(AutoResolveConstructorExpressionCache<TInstance>.Expression);
        }

        public StronglyTypedConfiguredBinding<TDependency1, TInstance> Register<TDependency1, TDependency2, TDependency3, TInstance>([CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
            where TInstance : class, TDependency1, TDependency2, TDependency3
        {
            var binding = new StronglyTypedBinding<TDependency1>(Register(new[] { typeof(TDependency1), typeof(TDependency2), typeof(TDependency3) }, callerFilePath, callerLineNumber));
            return binding.Inject<TInstance>(AutoResolveConstructorExpressionCache<TInstance>.Expression);
        }

        public StronglyTypedConfiguredBinding<TDependency1, TInstance> Register<TDependency1, TDependency2, TDependency3, TDependency4, TInstance>([CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
            where TInstance : class, TDependency1, TDependency2, TDependency3, TDependency4
        {
            var binding = new StronglyTypedBinding<TDependency1>(Register(new[] { typeof(TDependency1), typeof(TDependency2), typeof(TDependency3), typeof(TDependency4) }, callerFilePath, callerLineNumber));
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
            return _registrations.CreateBinding(new[] { dependencyType }, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Begins configuring a weakly typed binding for <paramref name="dependencyType"/>
        /// </summary>
        /// <param name="dependencyTypes"></param>
        /// <returns></returns>
#pragma warning disable 1573
        public WeaklyTypedBinding Register(Type[] dependencyTypes, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
#pragma warning restore 1573
        {
            return _registrations.CreateBinding(dependencyTypes, callerFilePath, callerLineNumber);
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
            dependencyType.CheckInstanceTypeIsAssignable(instanceType);
            WeaklyTypedBinding binding = Register(dependencyType, callerFilePath, callerLineNumber);
            Expression expression = AutoResolveExpression(instanceType);
            return binding.Inject(expression);
        }

        /// <summary>
        /// Registers a new weakly typed dependency and auto resolves a expression to create it.
        /// </summary>
        /// <param name="dependencyTypes"></param>
        /// <param name="instanceType"></param>
        /// <returns></returns>
#pragma warning disable 1573
        public WeaklyTypedConfiguredBinding Register(Type[] dependencyTypes, Type instanceType, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
#pragma warning restore 1573
        {
            dependencyTypes.CheckInstanceTypeIsAssignable(instanceType);
            WeaklyTypedBinding binding = Register(dependencyTypes, callerFilePath, callerLineNumber);
            Expression expression = AutoResolveExpression(instanceType);
            return binding.Inject(expression);
        }

        private Expression AutoResolveExpression(Type instanceType)
        {
            if (instanceType.ContainsGenericParameters)
            {
                return new OpenGenericTypeExpression(instanceType);
            }
            else
            {
                return instanceType.AutoResolveConstructorExpression();
            }
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

        internal ReadOnlyBindingConfig GetDependencies() => _registrations.GetDependencies();
    }
}