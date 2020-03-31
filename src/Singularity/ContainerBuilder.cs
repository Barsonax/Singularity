using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using Singularity.Collections;
using Singularity.Exceptions;
using Singularity.Expressions;

namespace Singularity
{
    /// <summary>
    /// A class to make configuring services easier
    /// </summary>
    public sealed class ContainerBuilder
    {
        internal SingularitySettings Settings { get; private set; }
        internal RegistrationStore Registrations { get; } = new RegistrationStore();

        /// <summary>
        /// Creates a new builder with the provided optional configurator and settings.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="settings"></param>
        public ContainerBuilder(Action<ContainerBuilder>? configurator = null, SingularitySettings? settings = null)
        {
            Settings = settings ?? SingularitySettings.Default;
            configurator?.Invoke(this);
        }

        /// <summary>
        /// Replaces the <see cref="SingularitySettings"/>
        /// </summary>
        /// <param name="settings"></param>
        public void ConfigureSettings(SingularitySettings settings)
        {
            Settings = settings;
        }

        /// <summary>
        /// Configures the <see cref="SingularitySettings"/>
        /// </summary>
        /// <param name="settings"></param>
        public void ConfigureSettings(Action<SingularitySettings> settings)
        {
            settings.Invoke(Settings);
        }

        /// <summary>
        /// Registers a new strongly typed binding.
        /// </summary>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public void Register<TImplementation>(Action<StronglyTypedServiceConfigurator<TImplementation>>? configurator = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
            where TImplementation : class
        {
            ServiceTypeValidator.Cache<TImplementation>.CheckIsEnumerable();
            var serviceTypes = SinglyLinkedListNodeTypeCache<TImplementation>.Instance;
            RegisterInternal(configurator, serviceTypes, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a new strongly typed binding.
        /// </summary>
        /// <typeparam name="TService1"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public void Register<TService1, TImplementation>(Action<StronglyTypedServiceConfigurator<TImplementation>>? configurator = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
            where TImplementation : class, TService1
        {
            ServiceTypeValidator.Cache<TImplementation>.CheckIsEnumerable();
            ServiceTypeValidator.Cache<TService1>.CheckIsEnumerable();
            var serviceTypes = SinglyLinkedListNodeTypeCache<TImplementation>.Instance
                                  .Add(typeof(TService1));
            RegisterInternal(configurator, serviceTypes, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a new strongly typed binding.
        /// </summary>
        /// <typeparam name="TService1"></typeparam>
        /// <typeparam name="TService2"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public void Register<TService1, TService2, TImplementation>(Action<StronglyTypedServiceConfigurator<TImplementation>>? configurator = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
            where TImplementation : class, TService1, TService2
        {
            ServiceTypeValidator.Cache<TImplementation>.CheckIsEnumerable();
            ServiceTypeValidator.Cache<TService1>.CheckIsEnumerable();
            ServiceTypeValidator.Cache<TService2>.CheckIsEnumerable();
            var serviceTypes = SinglyLinkedListNodeTypeCache<TImplementation>.Instance
                                  .Add(typeof(TService1))
                                  .Add(typeof(TService2));
            RegisterInternal(configurator, serviceTypes, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a new strongly typed binding.
        /// </summary>
        /// <typeparam name="TService1"></typeparam>
        /// <typeparam name="TService2"></typeparam>
        /// <typeparam name="TService3"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public void Register<TService1, TService2, TService3, TImplementation>(Action<StronglyTypedServiceConfigurator<TImplementation>>? configurator = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
            where TImplementation : class, TService1, TService2, TService3
        {
            ServiceTypeValidator.Cache<TImplementation>.CheckIsEnumerable();
            ServiceTypeValidator.Cache<TService1>.CheckIsEnumerable();
            ServiceTypeValidator.Cache<TService2>.CheckIsEnumerable();
            ServiceTypeValidator.Cache<TService3>.CheckIsEnumerable();
            var serviceTypes = SinglyLinkedListNodeTypeCache<TImplementation>.Instance
                                .Add(typeof(TService1))
                                .Add(typeof(TService2))
                                .Add(typeof(TService3));
            RegisterInternal(configurator, serviceTypes, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a new weakly typed binding.
        /// </summary>
        /// <param name="implementationType"></param>
        /// <param name="configurator"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public void Register(Type implementationType, Action<WeaklyTypedServiceConfigurator>? configurator = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            RegisterInternal(new SinglyLinkedListNode<Type>(implementationType), implementationType, configurator, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a new weakly typed binding.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <param name="configurator"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public void Register(Type serviceType, Type implementationType, Action<WeaklyTypedServiceConfigurator>? configurator = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            var serviceTypes = new SinglyLinkedListNode<Type>(implementationType)
                                    .Add(serviceType);
            RegisterInternal(serviceTypes, implementationType, configurator, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a new weakly typed binding.
        /// </summary>
        /// <param name="serviceTypes"></param>
        /// <param name="implementationType"></param>
        /// <param name="configurator"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public void Register(Type[] serviceTypes, Type implementationType, Action<WeaklyTypedServiceConfigurator>? configurator = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            var serviceTypes2 = new SinglyLinkedListNode<Type>(implementationType)
                                    .Add(serviceTypes);
            RegisterInternal(serviceTypes2, implementationType, configurator, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a batch of new weakly typed bindings.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationTypes"></param>
        /// <param name="configurator"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        public void Register(Type serviceType, Type[] implementationTypes, Action<WeaklyTypedServiceConfigurator>? configurator = null, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            foreach (Type implementationType in implementationTypes)
            {
                var serviceTypes = new SinglyLinkedListNode<Type>(implementationType)
                                    .Add(serviceType);
                RegisterInternal(serviceTypes, implementationType, configurator, callerFilePath, callerLineNumber);
            }
        }


        /// <summary>
        /// Registers a new strongly typed decorator for <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">The type to decorate</typeparam>
        /// <typeparam name="TDecorator">The type of the decorator</typeparam>
        /// <exception cref="InterfaceExpectedException">If <typeparamref name="TService"/> is not a interface</exception>
        /// <returns></returns>
        public void Decorate<TService, TDecorator>(Action<StronglyTypedDecoratorConfigurator<TService, TDecorator>>? configurator = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
            where TService : class
            where TDecorator : TService
        {
            DecorateInternal(configurator, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a new weakly typed decorator for <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="decoratorType"></param>
        /// <param name="configurator"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        public void Decorate(Type serviceType, Type decoratorType, Action<WeaklyTypedDecoratorConfigurator>? configurator = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            DecorateInternal(serviceType, decoratorType, configurator, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a batch of new weakly typed decorators for <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="decoratorTypes"></param>
        /// <param name="configurator"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        public void Decorate(Type serviceType, Type[] decoratorTypes, Action<WeaklyTypedDecoratorConfigurator>? configurator = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            foreach (Type decoratorType in decoratorTypes)
            {
                DecorateInternal(serviceType, decoratorType, configurator, callerFilePath, callerLineNumber);
            }
        }

        /// <summary>
        /// Registers a new strongly typed late injector for <typeparamref name="TInstance"/>
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <typeparam name="TInstance"></typeparam>
        public void LateInject<TInstance>(Action<StronglyTypedLateInjectorConfigurator<TInstance>>? configurator, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            LateInjectInternal(configurator, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a new strongly typed late injector for <paramref name="instanceType"/>
        /// </summary>
        /// <param name="instanceType"></param>
        /// <param name="configurator"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public void LateInject(Type instanceType, Action<WeaklyTypedLateInjectorConfigurator>? configurator, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            LateInjectInternal(instanceType, configurator, callerFilePath, callerLineNumber);
        }

        private void RegisterInternal<TImplementation>(Action<StronglyTypedServiceConfigurator<TImplementation>>? configurator, SinglyLinkedListNode<Type> serviceTypes, string callerFilePath, int callerLineNumber)
        {
            var metadata = new BindingMetadata(callerFilePath, callerLineNumber, Registrations.CurrentModule);
            var context = new StronglyTypedServiceConfigurator<TImplementation>(metadata, serviceTypes, Settings);
            configurator?.Invoke(context);
            ServiceBinding serviceBinding = context.ToBinding();
            Registrations.AddBinding(serviceBinding);
        }

        private void RegisterInternal(SinglyLinkedListNode<Type> serviceTypes, Type implementationType, Action<WeaklyTypedServiceConfigurator>? configurator, string callerFilePath, int callerLineNumber)
        {
            var metadata = new BindingMetadata(callerFilePath, callerLineNumber, Registrations.CurrentModule);
            var context = new WeaklyTypedServiceConfigurator(serviceTypes, implementationType, metadata, Settings);
            configurator?.Invoke(context);
            ServiceBinding serviceBinding = context.ToBinding();
            Registrations.AddBinding(serviceBinding);
        }

        private void DecorateInternal<TService, TDecorator>(Action<StronglyTypedDecoratorConfigurator<TService, TDecorator>>? configurator, string callerFilePath, int callerLineNumber)
            where TService : class
            where TDecorator : TService
        {
            var metadata = new BindingMetadata(callerFilePath, callerLineNumber, Registrations.CurrentModule);
            var context = new StronglyTypedDecoratorConfigurator<TService, TDecorator>(metadata, Settings);
            configurator?.Invoke(context);
            Expression binding = context.ToBinding();
            Registrations.AddDecorator(typeof(TService), binding);
        }

        private void DecorateInternal(Type serviceType, Type decoratorType, Action<WeaklyTypedDecoratorConfigurator>? configurator, string callerFilePath, int callerLineNumber)
        {
            var metadata = new BindingMetadata(callerFilePath, callerLineNumber, Registrations.CurrentModule);
            var context = new WeaklyTypedDecoratorConfigurator(serviceType, decoratorType, metadata, Settings);
            configurator?.Invoke(context);
            Expression binding = context.ToBinding();
            Registrations.AddDecorator(serviceType, binding);
        }

        private void LateInjectInternal<TInstance>(Action<StronglyTypedLateInjectorConfigurator<TInstance>>? configurator, string callerFilePath, int callerLineNumber)
        {
            var metadata = new BindingMetadata(callerFilePath, callerLineNumber, Registrations.CurrentModule);
            var context = new StronglyTypedLateInjectorConfigurator<TInstance>(metadata);
            configurator?.Invoke(context);
            LateInjectorBinding binding = context.ToBinding();
            Registrations.AddLateInjectorBinding(binding);
        }

        private void LateInjectInternal(Type instanceType, Action<WeaklyTypedLateInjectorConfigurator>? configurator, string callerFilePath, int callerLineNumber)
        {
            var metadata = new BindingMetadata(callerFilePath, callerLineNumber, Registrations.CurrentModule);
            var context = new WeaklyTypedLateInjectorConfigurator(instanceType, metadata);
            configurator?.Invoke(context);
            LateInjectorBinding binding = context.ToBinding();
            Registrations.AddLateInjectorBinding(binding);
        }
    }
}