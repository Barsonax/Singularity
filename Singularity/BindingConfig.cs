using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Singularity.Bindings;
using Singularity.Exceptions;
using Singularity.Expressions;

namespace Singularity
{
    /// <summary>
    /// A class to make configuring dependencies easier
    /// </summary>
    public sealed class BindingConfig
    {
        /// <summary>
        /// If true then this config is locked and may not be modified.
        /// </summary>
        public bool Locked => _readonlyBindings != null;
        internal IModule? CurrentModule;
        private readonly Dictionary<Type, Registration> _registrations = new Dictionary<Type, Registration>();
        private ReadOnlyCollection<ReadonlyRegistration>? _readonlyBindings;

        /// <summary>
        /// Begins configuring a strongly typed binding for <typeparamref name="TDependency"/>
        /// </summary>
        /// <typeparam name="TDependency"></typeparam>
        /// <returns></returns>
        public StronglyTypedBinding<TDependency> Register<TDependency>([CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            if (Locked) throw new BindingConfigException("This config is locked and cannot be modified anymore!");
            return CreateBinding<TDependency>(callerFilePath, callerLineNumber);
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
            if (Locked) throw new BindingConfigException("This config is locked and cannot be modified anymore!");
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
            if (Locked) throw new BindingConfigException("This config is locked and cannot be modified anymore!");
            return CreateBinding(dependencyType, callerFilePath, callerLineNumber);
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
            if (Locked) throw new BindingConfigException("This config is locked and cannot be modified anymore!");
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
            if (Locked) throw new BindingConfigException("This config is locked and cannot be modified anymore!");
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
            if (Locked) throw new BindingConfigException("This config is locked and cannot be modified anymore!");

            var decorator = new StronglyTypedDecoratorBinding<TDependency>(AutoResolveConstructorExpressionCache<TDecorator>.Expression);

            ParameterExpression[] parameters = decorator.Expression.GetParameterExpressions();
            if (parameters.All(x => x.Type != typeof(TDependency))) throw new InvalidExpressionArgumentsException($"Cannot decorate {typeof(TDependency)} since the expression to create {typeof(TDecorator)} does not have a parameter for {typeof(TDependency)}");
            Registration registration = GetOrCreateRegistration(typeof(TDependency));
            registration.DecoratorBindings.Add(decorator);
            return decorator;
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
            if (Locked) throw new BindingConfigException("This config is locked and cannot be modified anymore!");

            TypeInfo typeInfo = decoratorType.GetTypeInfo();
            if (!dependencyType.GetTypeInfo().IsAssignableFrom(typeInfo)) throw new TypeNotAssignableException($"{dependencyType} is not implemented by {decoratorType}");

            var decorator = new WeaklyTypedDecoratorBinding(dependencyType, decoratorType.AutoResolveConstructorExpression());

            ParameterExpression[] parameters = decorator.Expression.GetParameterExpressions();
            if (parameters.All(x => x.Type != dependencyType)) throw new InvalidExpressionArgumentsException($"Cannot decorate {dependencyType} since the expression to create {decoratorType} does not have a parameter for {dependencyType}");
            Registration registration = GetOrCreateRegistration(dependencyType);
            registration.DecoratorBindings.Add(decorator);
            return decorator;
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
            if (Locked) throw new BindingConfigException("This config is locked and cannot be modified anymore!");

            var bindings = new WeaklyTypedDecoratorBinding[decoratorTypes.Length];
            for (var index = 0; index < decoratorTypes.Length; index++)
            {
                Type decoratorType = decoratorTypes[index];
                bindings[index] = Decorate(dependencyType, decoratorType);
            }

            return new WeaklyTypedDecoratorBindingBatch(new ReadOnlyCollection<WeaklyTypedDecoratorBinding>(bindings));
        }

        internal ReadOnlyCollection<ReadonlyRegistration> GetDependencies()
        {
            if (_readonlyBindings == null)
            {
                var readonlyRegistrations = new ReadonlyRegistration[_registrations.Count];
                var count = 0;
                foreach (var registration in _registrations.Values)
                {
                    registration.Verify();

                    readonlyRegistrations[count] = new ReadonlyRegistration(registration);
                    count++;
                }

                _readonlyBindings = new ReadOnlyCollection<ReadonlyRegistration>(readonlyRegistrations);
            }
            return _readonlyBindings!;
        }

        private Registration GetOrCreateRegistration(Type type)
        {
            if (!_registrations.TryGetValue(type, out Registration registration))
            {
                registration = new Registration(type);
                _registrations.Add(type, registration);
            }

            return registration;
        }

        private StronglyTypedBinding<TDependency> CreateBinding<TDependency>(string callerFilePath, int callerLineNumber)
        {
            Registration registration = GetOrCreateRegistration(typeof(TDependency));
            var binding = new StronglyTypedBinding<TDependency>(callerFilePath, callerLineNumber, CurrentModule);
            registration.Bindings.Add(binding);
            return binding;
        }

        private WeaklyTypedBinding CreateBinding(Type dependencyType, string callerFilePath, int callerLineNumber)
        {
            Registration registration = GetOrCreateRegistration(dependencyType);
            var binding = new WeaklyTypedBinding(dependencyType, callerFilePath, callerLineNumber, CurrentModule);
            registration.Bindings.Add(binding);
            return binding;
        }
    }
}