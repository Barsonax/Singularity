using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Singularity.Bindings;
using Singularity.Exceptions;

namespace Singularity
{
    /// <summary>
    /// A class to make configuring dependencies easier
    /// </summary>
    public sealed class BindingConfig
    {
        public bool Verified => _readonlyBindings != null;
        internal IModule? CurrentModule;
        private readonly Dictionary<Type, WeaklyTypedBinding> _bindings = new Dictionary<Type, WeaklyTypedBinding>();
        private ReadOnlyCollection<Binding>? _readonlyBindings;

        /// <summary>
        /// Begins configuring a strongly typed binding for <typeparamref name="TDependency"/>
        /// </summary>
        /// <typeparam name="TDependency"></typeparam>
        /// <returns></returns>
        public StronglyTypedBinding<TDependency> Register<TDependency>([CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            if (Verified) throw new BindingConfigException("This config is locked and cannot be modified anymore!");
            return GetOrCreateBinding<TDependency>(callerFilePath, callerLineNumber);
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
            if (Verified) throw new BindingConfigException("This config is locked and cannot be modified anymore!");
            StronglyTypedBinding<TDependency> binding = Register<TDependency>(callerFilePath, callerLineNumber);
            return binding.Inject<TInstance>(AutoResolveConstructorExpressionCache<TInstance>.Expression);
        }

        /// <summary>
        /// Begins configuring a weakly typed binding for <paramref name="instanceType"/>
        /// </summary>
        /// <param name="instanceType"></param>
        /// <returns></returns>
#pragma warning disable 1573
        public WeaklyTypedBinding Register(Type instanceType, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
#pragma warning restore 1573
        {
            if (Verified) throw new BindingConfigException("This config is locked and cannot be modified anymore!");
            MethodInfo registerMethod = (from m in typeof(BindingConfig).GetRuntimeMethods()
                                         where m.Name == nameof(Register)
                                         where m.IsGenericMethod
                                         where m.GetGenericArguments().Length == 1
                                         select m).First().MakeGenericMethod(instanceType);

            return (WeaklyTypedBinding)registerMethod.Invoke(this, new object[] { callerFilePath, callerLineNumber });
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
            if (Verified) throw new BindingConfigException("This config is locked and cannot be modified anymore!");
            MethodInfo registerMethod = (from m in typeof(BindingConfig).GetRuntimeMethods()
                                         where m.Name == nameof(Register)
                                         where m.IsGenericMethod
                                         where m.GetGenericArguments().Length == 2
                                         select m).First().MakeGenericMethod(dependencyType, instanceType);

            return (WeaklyTypedConfiguredBinding)registerMethod.Invoke(this, new object[] { callerFilePath, callerLineNumber });
        }

        /// <summary>
        /// Begins configuring a strongly typed decorator for <typeparamref name="TDependency"/>.
        /// </summary>
        /// <typeparam name="TDependency">The type to decorate</typeparam>
        /// <exception cref="InterfaceExpectedException">If <typeparamref name="TDependency"/> is not a interface</exception>
        /// <returns></returns>
        public StronglyTypedDecoratorBinding<TDependency> Decorate<TDependency>([CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
            where TDependency : class
        {
            if (Verified) throw new BindingConfigException("This config is locked and cannot be modified anymore!");
            var decorator = new StronglyTypedDecoratorBinding<TDependency>();
            StronglyTypedBinding<TDependency> binding = GetOrCreateBinding<TDependency>(callerFilePath, callerLineNumber);
            binding.AddDecorator(decorator);
            return decorator;
        }

        /// <summary>
        /// Begins configuring a weakly typed decorator for <paramref name="dependencyType"/>
        /// </summary>
        /// <param name="dependencyType"></param>
        /// <returns></returns>
#pragma warning disable 1573
        public WeaklyTypedDecoratorBinding Decorate(Type dependencyType, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
#pragma warning restore 1573
        {
            if (Verified) throw new BindingConfigException("This config is locked and cannot be modified anymore!");
            MethodInfo decorateMethod = (from m in typeof(BindingConfig).GetRuntimeMethods()
                                         where m.Name == nameof(Decorate)
                                         where m.IsGenericMethod
                                         where m.GetGenericArguments().Length == 1
                                         select m).First().MakeGenericMethod(dependencyType);

            return (WeaklyTypedDecoratorBinding)decorateMethod.Invoke(this, new object[] { callerFilePath, callerLineNumber });
        }

        internal ReadOnlyCollection<Binding> GetDependencies()
        {
            if (_readonlyBindings == null)
            {
                var readonlyBindings = new Binding[_bindings.Count];

                var index = 0;
                foreach (KeyValuePair<Type, WeaklyTypedBinding> weaklyTypedBinding in _bindings)
                {
                    weaklyTypedBinding.Value.Verify();
                    readonlyBindings[index] = new Binding(weaklyTypedBinding.Value);
                    index++;
                }
                _readonlyBindings = new ReadOnlyCollection<Binding>(readonlyBindings);
            }
            return _readonlyBindings!;
        }

        private StronglyTypedBinding<TDependency> GetOrCreateBinding<TDependency>(string callerFilePath, int callerLineNumber)
        {
            if (_bindings.TryGetValue(typeof(TDependency), out WeaklyTypedBinding weaklyTypedBinding))
            {
                return (StronglyTypedBinding<TDependency>)weaklyTypedBinding;
            }
            var binding = new StronglyTypedBinding<TDependency>(callerFilePath, callerLineNumber, CurrentModule);
            _bindings.Add(binding.DependencyType, binding);
            return binding;
        }
    }
}