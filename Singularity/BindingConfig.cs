using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Singularity.Bindings;
using Singularity.Exceptions;
using Singularity.Graph;

namespace Singularity
{
    /// <summary>
    /// A class to make configuring dependencies easier
    /// </summary>
    public sealed class BindingConfig : IBindingConfig
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
            return binding.SetExpression<TInstance>(typeof(TInstance).AutoResolveConstructorExpression());
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

        /// <summary>
        /// Implementation of <see cref="IEnumerable{Binding}.GetEnumerator"/>
        /// </summary>
        /// <returns></returns>
		public IEnumerator<Binding> GetEnumerator()
        {
            foreach (var binding in Bindings.Values)
            {
                if (binding.Expression == null && binding.Decorators == null) throw new BindingConfigException($"The binding at {binding.BindingMetadata.GetPosition()} does not have a expression");
                DecoratorBinding[] decorators;
                if (binding.Decorators != null)
                {
                    decorators = new DecoratorBinding[binding.Decorators.Count];
                    for (var i = 0; i < binding.Decorators.Count; i++)
                    {
                        IDecoratorBinding decorator = binding.Decorators[i];
                        if (decorator.Expression == null) throw new BindingConfigException($"The decorator for {decorator.DependencyType} does not have a expression");
                        decorators[i] = new DecoratorBinding(decorator.Expression);
                    }
                }
                else
                {
                    decorators = new DecoratorBinding[0];
                }


                yield return new Binding(binding.BindingMetadata, binding.DependencyType, binding.Expression, binding.Lifetime, decorators, binding.OnDeathAction);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private StronglyTypedBinding<TDependency> GetOrCreateBinding<TDependency>(string callerFilePath, int callerLineNumber)
        {
            if (Bindings.TryGetValue(typeof(TDependency), out IBinding weaklyTypedBinding))
            {
                return (StronglyTypedBinding<TDependency>)weaklyTypedBinding;
            }
            var binding = new StronglyTypedBinding<TDependency>(callerFilePath, callerLineNumber, CurrentModule);
            AddBinding(binding);
            return binding;
        }

        private void AddBinding(IBinding binding)
        {
            _bindings.Add(binding.DependencyType, binding);
        }
    }
}