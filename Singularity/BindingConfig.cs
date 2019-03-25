using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
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
        internal IReadOnlyDictionary<Type, IBinding> Bindings => _bindings;
        internal IModule? CurrentModule;
        private readonly Dictionary<Type, IBinding> _bindings = new Dictionary<Type, IBinding>();

        /// <summary>
        /// Registers a new dependency and auto resolves a expression to create it.
        /// </summary>
        /// <typeparam name="TDependency"></typeparam>
        /// <typeparam name="TInstance"></typeparam>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        public StronglyTypedConfiguredBinding<TDependency, TInstance> Register<TDependency, TInstance>([CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
            where TInstance : class, TDependency
        {
            var binding = Register<TDependency>(callerFilePath, callerLineNumber);
            return binding.SetExpression<TInstance>(AutoResolveConstructorExpressionCache<TInstance>.Expression);
        }

        /// <summary>
        /// Begins configuring a binding for a certain dependency
        /// </summary>
        /// <typeparam name="TDependency"></typeparam>
        /// <returns></returns>
        public StronglyTypedBinding<TDependency> Register<TDependency>([CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            return GetOrCreateBinding<TDependency>(callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Registers a new weakly typed dependency and auto resolves a expression to create it.
        /// </summary>
        /// <param name="dependencyType"></param>
        /// <param name="instanceType"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        public WeaklyTypedBinding Register(Type dependencyType, Type instanceType, [CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            var newExpression = instanceType.AutoResolveConstructorExpression();
            var binding = new WeaklyTypedBinding(dependencyType, newExpression, callerFilePath, callerLineNumber, CurrentModule);
            _bindings.Add(dependencyType, binding);
            return binding;
        }

        /// <summary>
        /// Begins configuring a decorator to for <typeparamref name="TDependency"/>.
        /// </summary>
        /// <typeparam name="TDependency">The type to decorate</typeparam>
        /// <exception cref="InterfaceExpectedException">If <typeparamref name="TDependency"/> is not a interface</exception>
        /// <returns></returns>
        public StronglyTypedDecoratorBinding<TDependency> Decorate<TDependency>([CallerFilePath]string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
            where TDependency : class
        {
            var decorator = new StronglyTypedDecoratorBinding<TDependency>();
            StronglyTypedBinding<TDependency> binding = GetOrCreateBinding<TDependency>(callerFilePath, callerLineNumber);
            if (binding.Decorators == null) binding.Decorators = new List<IDecoratorBinding>();
            binding.Decorators.Add(decorator);
            return decorator;
        }

        internal ReadOnlyDictionary<Type, Dependency> GetDependencies()
        {
            var dictionary = new Dictionary<Type, Dependency>(_bindings.Count);
            foreach (KeyValuePair<Type, IBinding> binding in _bindings)
            {
                if (binding.Value.Expression == null && binding.Value.Decorators == null) throw new BindingConfigException($"The binding at {binding.Value.BindingMetadata.GetPosition()} does not have a expression");
                Expression[] decorators;
                if (binding.Value.Decorators != null)
                {
                    decorators = new Expression[binding.Value.Decorators.Count];
                    for (var i = 0; i < binding.Value.Decorators.Count; i++)
                    {
                        IDecoratorBinding decorator = binding.Value.Decorators[i];
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
            if (_bindings.TryGetValue(typeof(TDependency), out IBinding weaklyTypedBinding))
            {
                return (StronglyTypedBinding<TDependency>)weaklyTypedBinding;
            }
            var binding = new StronglyTypedBinding<TDependency>(callerFilePath, callerLineNumber, CurrentModule);
            _bindings.Add(binding.DependencyType, binding);
            return binding;
        }
    }
}