using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Singularity.Collections;
using Singularity.Exceptions;
using Singularity.Expressions;
using Singularity.Graph;

namespace Singularity
{
    /// <summary>
    /// A class to make configuring dependencies easier
    /// </summary>
    public sealed class ContainerBuilder
    {
        /// <summary>
        /// The container which is being build.
        /// </summary>
        public Container Container { get; }

        internal RegistrationStore Registrations { get; } = new RegistrationStore();

        internal ContainerBuilder(Container container)
        {
            Container = container;
        }

        /// <summary>
        /// Registers a new strongly typed binding.
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="constructorSelector"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public void Register<TInstance>(Action<StronglyTypedServiceConfigurator<TInstance, TInstance>>? configurator = null, IConstructorSelector? constructorSelector = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
            where TInstance : class
        {
            RegisterInternal(configurator, constructorSelector, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a new strongly typed binding.
        /// </summary>
        /// <typeparam name="TDependency"></typeparam>
        /// <typeparam name="TInstance"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="constructorSelector"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public void Register<TDependency, TInstance>(Action<StronglyTypedServiceConfigurator<TDependency, TInstance>>? configurator = null, IConstructorSelector? constructorSelector = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
            where TInstance : class, TDependency
        {
            RegisterInternal(configurator, constructorSelector, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a new weakly typed binding.
        /// </summary>
        /// <param name="instanceType"></param>
        /// <param name="configurator"></param>
        /// <param name="constructorSelector"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public void Register(Type instanceType, Action<WeaklyTypedServiceConfigurator>? configurator = null, IConstructorSelector? constructorSelector = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            RegisterInternal(instanceType, instanceType, configurator, constructorSelector, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a new weakly typed binding.
        /// </summary>
        /// <param name="dependencyType"></param>
        /// <param name="instanceType"></param>
        /// <param name="configurator"></param>
        /// <param name="constructorSelector"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public void Register(Type dependencyType, Type instanceType, Action<WeaklyTypedServiceConfigurator>? configurator = null, IConstructorSelector? constructorSelector = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            RegisterInternal(dependencyType, instanceType, configurator, constructorSelector, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a batch of new weakly typed bindings.
        /// </summary>
        /// <param name="dependencyType"></param>
        /// <param name="instanceTypes"></param>
        /// <param name="configurator"></param>
        /// <param name="constructorSelector"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        public void Register(Type dependencyType, Type[] instanceTypes, Action<WeaklyTypedServiceConfigurator>? configurator = null, IConstructorSelector? constructorSelector = null, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            foreach (Type instanceType in instanceTypes)
            {
                RegisterInternal(dependencyType, instanceType, configurator, constructorSelector, callerFilePath, callerLineNumber);
            }
        }


        /// <summary>
        /// Registers a new strongly typed decorator for <typeparamref name="TDependency"/>.
        /// </summary>
        /// <typeparam name="TDependency">The type to decorate</typeparam>
        /// <typeparam name="TDecorator">The type of the decorator</typeparam>
        /// <exception cref="InterfaceExpectedException">If <typeparamref name="TDependency"/> is not a interface</exception>
        /// <returns></returns>
        public void Decorate<TDependency, TDecorator>(Action<StronglyTypedDecoratorConfigurator<TDependency, TDecorator>>? configurator = null, IConstructorSelector? constructorSelector = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
            where TDependency : class
            where TDecorator : TDependency
        {
            DecorateInternal(configurator, constructorSelector, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a new weakly typed decorator for <paramref name="dependencyType"/>
        /// </summary>
        /// <param name="dependencyType"></param>
        /// <param name="decoratorType"></param>
        /// <param name="configurator"></param>
        /// <param name="constructorSelector"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        public void Decorate(Type dependencyType, Type decoratorType, Action<WeaklyTypedDecoratorConfigurator>? configurator = null, IConstructorSelector? constructorSelector = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            DecorateInternal(dependencyType, decoratorType, configurator, constructorSelector, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a batch of new weakly typed decorators for <paramref name="dependencyType"/>.
        /// </summary>
        /// <param name="dependencyType"></param>
        /// <param name="decoratorTypes"></param>
        /// <param name="configurator"></param>
        /// <param name="constructorSelector"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        public void Decorate(Type dependencyType, Type[] decoratorTypes, Action<WeaklyTypedDecoratorConfigurator>? configurator = null, IConstructorSelector? constructorSelector = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            foreach (Type decoratorType in decoratorTypes)
            {
                DecorateInternal(dependencyType, decoratorType, configurator, constructorSelector, callerFilePath, callerLineNumber);
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

        private void RegisterInternal<TDependency, TInstance>(Action<StronglyTypedServiceConfigurator<TDependency, TInstance>>? configurator, IConstructorSelector? constructorSelector, string callerFilePath, int callerLineNumber)
            where TInstance : class, TDependency
        {
            var metadata = new BindingMetadata(callerFilePath, callerLineNumber, Registrations.CurrentModule);
            var context = new StronglyTypedServiceConfigurator<TDependency, TInstance>(metadata, Container.Options, constructorSelector);
            configurator?.Invoke(context);
            ServiceBinding serviceBinding = context.ToBinding();
            Registrations.AddBinding(serviceBinding);
        }

        private void RegisterInternal(Type dependencyType, Type instanceType, Action<WeaklyTypedServiceConfigurator>? configurator, IConstructorSelector? constructorSelector, string callerFilePath, int callerLineNumber)
        {
            var metadata = new BindingMetadata(callerFilePath, callerLineNumber, Registrations.CurrentModule);
            var context = new WeaklyTypedServiceConfigurator(dependencyType, instanceType, metadata, Container.Options, constructorSelector);
            configurator?.Invoke(context);
            ServiceBinding serviceBinding = context.ToBinding();
            Registrations.AddBinding(serviceBinding);
        }

        private void DecorateInternal<TDependency, TDecorator>(Action<StronglyTypedDecoratorConfigurator<TDependency, TDecorator>>? configurator, IConstructorSelector? constructorSelector, string callerFilePath, int callerLineNumber)
            where TDependency : class
            where TDecorator : TDependency
        {
            var metadata = new BindingMetadata(callerFilePath, callerLineNumber, Registrations.CurrentModule);
            var context = new StronglyTypedDecoratorConfigurator<TDependency, TDecorator>(metadata, Container.Options, constructorSelector);
            configurator?.Invoke(context);
            Expression binding = context.ToBinding();
            Registrations.AddDecorator(typeof(TDependency), binding);
        }

        private void DecorateInternal(Type dependencyType, Type decoratorType, Action<WeaklyTypedDecoratorConfigurator>? configurator, IConstructorSelector? constructorSelector, string callerFilePath, int callerLineNumber)
        {
            var metadata = new BindingMetadata(callerFilePath, callerLineNumber, Registrations.CurrentModule);
            var context = new WeaklyTypedDecoratorConfigurator(dependencyType, decoratorType, metadata, Container.Options, constructorSelector);
            configurator?.Invoke(context);
            Expression binding = context.ToBinding();
            Registrations.AddDecorator(dependencyType, binding);
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