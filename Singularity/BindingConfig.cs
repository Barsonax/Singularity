using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Singularity.Bindings;
using Singularity.Exceptions;
using Singularity.Graph;

namespace Singularity
{
    /// <summary>
    /// A class to make configuring dependencies easier
    /// </summary>
    public sealed class BindingConfig
    {
        internal IReadOnlyDictionary<Type, WeaklyTypedBinding> Bindings => _bindings;
        internal IModule? CurrentModule;
        private readonly Dictionary<Type, WeaklyTypedBinding> _bindings = new Dictionary<Type, WeaklyTypedBinding>();

        /// <summary>
        /// Begins configuring a strongly typed binding for <typeparamref name="TDependency"/>
        /// </summary>
        /// <typeparam name="TDependency"></typeparam>
        /// <returns></returns>
        public StronglyTypedBinding<TDependency> Register<TDependency>([CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
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
            var decorator = new StronglyTypedDecoratorBinding<TDependency>();
            StronglyTypedBinding<TDependency> binding = GetOrCreateBinding<TDependency>(callerFilePath, callerLineNumber);
            if (binding.Decorators == null) binding.Decorators = new List<WeaklyTypedDecoratorBinding>();
            binding.Decorators.Add(decorator);
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
            MethodInfo decorateMethod = (from m in typeof(BindingConfig).GetRuntimeMethods()
                                         where m.Name == nameof(Decorate)
                                         where m.IsGenericMethod
                                         where m.GetGenericArguments().Length == 1
                                         select m).First().MakeGenericMethod(dependencyType);

            return (WeaklyTypedDecoratorBinding)decorateMethod.Invoke(this, new object[] { callerFilePath, callerLineNumber });
        }

        internal ReadOnlyDictionary<Type, Dependency> GetDependencies()
        {
            var dictionary = new Dictionary<Type, Dependency>(_bindings.Count);
            foreach (KeyValuePair<Type, WeaklyTypedBinding> binding in _bindings)
            {
                if (binding.Value.Expression == null && binding.Value.Decorators == null) throw new BindingConfigException($"The binding at {binding.Value.BindingMetadata.GetPosition()} does not have a expression");
                Expression[] decorators;
                if (binding.Value.Decorators != null)
                {
                    decorators = new Expression[binding.Value.Decorators.Count];
                    for (var i = 0; i < binding.Value.Decorators.Count; i++)
                    {
                        WeaklyTypedDecoratorBinding decorator = binding.Value.Decorators[i];
                        if (decorator.Expression == null) throw new BindingConfigException($"The decorator for {binding.Value.DependencyType} does not have a expression");
                        decorators[i] = decorator.Expression;
                    }
                }
                else
                {
                    decorators = new Expression[0];
                }
               ;
                dictionary.Add(binding.Key,
                    new Dependency(
                        new Binding(binding.Value.BindingMetadata, binding.Value.DependencyType, binding.Value.Expression, binding.Value.Lifetime, decorators, binding.Value.OnDeathAction)));

            }
            return new ReadOnlyDictionary<Type, Dependency>(dictionary);
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