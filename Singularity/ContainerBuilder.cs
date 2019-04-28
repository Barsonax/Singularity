using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Singularity.Collections;
using Singularity.Exceptions;

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
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public void Register<TInstance>(Action<StronglyTypedServiceConfigurator<TInstance, TInstance>>? configurator = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
            where TInstance : class
        {
            RegisterInternal(configurator, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a new strongly typed binding.
        /// </summary>
        /// <typeparam name="TDependency"></typeparam>
        /// <typeparam name="TInstance"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public void Register<TDependency, TInstance>(Action<StronglyTypedServiceConfigurator<TDependency, TInstance>>? configurator = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
            where TInstance : class, TDependency
        {
            RegisterInternal(configurator, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a new weakly typed binding.
        /// </summary>
        /// <param name="instanceType"></param>
        /// <param name="configurator"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public void Register(Type instanceType, Action<WeaklyTypedServiceConfigurator>? configurator = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            RegisterInternal(instanceType, instanceType, configurator, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a new weakly typed binding.
        /// </summary>
        /// <param name="dependencyType"></param>
        /// <param name="instanceType"></param>
        /// <param name="configurator"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public void Register(Type dependencyType, Type instanceType, Action<WeaklyTypedServiceConfigurator>? configurator = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            RegisterInternal(dependencyType, instanceType, configurator, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a batch of new weakly typed bindings.
        /// </summary>
        /// <param name="dependencyType"></param>
        /// <param name="instanceTypes"></param>
        /// <param name="configurator"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        public void Register(Type dependencyType, Type[] instanceTypes, Action<WeaklyTypedServiceConfigurator>? configurator = null, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            foreach (Type instanceType in instanceTypes)
            {
                RegisterInternal(dependencyType, instanceType, configurator, callerFilePath, callerLineNumber);
            }
        }


        /// <summary>
        /// Registers a new strongly typed decorator for <typeparamref name="TDependency"/>.
        /// </summary>
        /// <typeparam name="TDependency">The type to decorate</typeparam>
        /// <typeparam name="TDecorator">The type of the decorator</typeparam>
        /// <exception cref="InterfaceExpectedException">If <typeparamref name="TDependency"/> is not a interface</exception>
        /// <returns></returns>
        public void Decorate<TDependency, TDecorator>(Action<StronglyTypedDecoratorConfigurator<TDependency, TDecorator>>? configurator = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
            where TDependency : class
            where TDecorator : TDependency
        {
            DecorateInternal(configurator, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a new weakly typed decorator for <paramref name="dependencyType"/>
        /// </summary>
        /// <param name="dependencyType"></param>
        /// <param name="decoratorType"></param>
        /// <param name="configurator"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        public void Decorate(Type dependencyType, Type decoratorType, Action<WeaklyTypedDecoratorConfigurator>? configurator = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            DecorateInternal(dependencyType, decoratorType, configurator, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a batch of new weakly typed decorators for <paramref name="dependencyType"/>.
        /// </summary>
        /// <param name="dependencyType"></param>
        /// <param name="decoratorTypes"></param>
        /// <param name="configurator"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        public void Decorate(Type dependencyType, Type[] decoratorTypes, Action<WeaklyTypedDecoratorConfigurator>? configurator = null, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            foreach (Type decoratorType in decoratorTypes)
            {
                DecorateInternal(dependencyType, decoratorType, configurator, callerFilePath, callerLineNumber);
            }
        }

        private void RegisterInternal<TDependency, TInstance>(Action<StronglyTypedServiceConfigurator<TDependency, TInstance>>? configurator = null, string callerFilePath = "", int callerLineNumber = -1)
            where TInstance : class, TDependency
        {
            var context = new StronglyTypedServiceConfigurator<TDependency, TInstance>(callerFilePath, callerLineNumber, Registrations.CurrentModule);
            configurator?.Invoke(context);
            Binding binding = context.ToBinding();
            Registrations.AddBinding(binding);
        }

        private void RegisterInternal(Type dependencyType, Type instanceType, Action<WeaklyTypedServiceConfigurator>? configurator = null, string callerFilePath = "", int callerLineNumber = -1)
        {
            var context = new WeaklyTypedServiceConfigurator(dependencyType, instanceType, callerFilePath, callerLineNumber, Registrations.CurrentModule);
            configurator?.Invoke(context);
            Binding binding = context.ToBinding();
            Registrations.AddBinding(binding);
        }

        private void DecorateInternal<TDependency, TDecorator>(Action<StronglyTypedDecoratorConfigurator<TDependency, TDecorator>>? registrator = null, string callerFilePath = "", int callerLineNumber = -1)
            where TDependency : class
            where TDecorator : TDependency
        {
            var context = new StronglyTypedDecoratorConfigurator<TDependency, TDecorator>(callerFilePath, callerLineNumber, Registrations.CurrentModule);
            registrator?.Invoke(context);
            Expression binding = context.ToBinding();
            Registrations.AddDecorator(typeof(TDependency), binding);
        }

        private void DecorateInternal(Type dependencyType, Type decoratorType, Action<WeaklyTypedDecoratorConfigurator>? configurator = null, string callerFilePath = "", int callerLineNumber = -1)
        {
            var context = new WeaklyTypedDecoratorConfigurator(dependencyType, decoratorType, callerFilePath, callerLineNumber, Registrations.CurrentModule);
            configurator?.Invoke(context);
            Expression binding = context.ToBinding();
            Registrations.AddDecorator(dependencyType, binding);
        }
    }
}