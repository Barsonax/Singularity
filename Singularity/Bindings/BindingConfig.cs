using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Singularity.Exceptions;
using Singularity.Graph;

namespace Singularity.Bindings
{
    public sealed class BindingConfig : IBindingConfig
	{
		internal IReadOnlyDictionary<Type, IBinding> Bindings => _bindings;
		internal IModule? CurrentModule;
		private readonly Dictionary<Type, IBinding> _bindings = new Dictionary<Type, IBinding>();

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
			binding.Decorators.Add(decorator);
			return decorator;
		}

		public IEnumerator<Binding> GetEnumerator()
		{
            foreach (var binding in Bindings.Values)
            {
                if (binding.Expression == null && binding.Decorators.Count == 0) throw new BindingConfigException($"The binding at {binding.BindingMetadata.GetPosition()} does not have a expression");
                var decorators = new List<DecoratorBinding>();
                foreach (var decorator in binding.Decorators)
                {
                    if (decorator.Expression == null) throw new BindingConfigException($"The decorator for {decorator.DependencyType} does not have a expression");
                    decorators.Add(new DecoratorBinding(decorator.Expression));
                }
                yield return new Binding(binding.BindingMetadata, binding.DependencyType, binding.Expression, binding.Lifetime, decorators, binding.OnDeath);
            }
            //return Bindings.Values.Select(x => new Binding(x.BindingMetadata, x.DependencyType, x.Expression, x.Lifetime, x.Decorators, x.OnDeath)).GetEnumerator();
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