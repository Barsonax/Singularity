using System;
using System.Collections;
using System.Collections.Generic;

using Singularity.Exceptions;

namespace Singularity.Bindings
{
	public class BindingConfig : IBindingConfig
	{
        private readonly Dictionary<Type, IBinding> _bindings = new Dictionary<Type, IBinding>();

        public IReadOnlyDictionary<Type, IBinding> Bindings => _bindings;

        /// <summary>
        /// Begins configuring a binding for a certain dependency
        /// </summary>
        /// <typeparam name="TDependency"></typeparam>
        /// <returns></returns>
        public StronglyTypedBinding<TDependency> For<TDependency>()
        {
            return GetOrCreateBinding<TDependency>();
        }

        /// <summary>
        /// Begins configuring a decorator to for <see cref="TDependency"/>.
        /// </summary>
        /// <typeparam name="TDependency">The type to decorate</typeparam>
        /// <exception cref="InterfaceExpectedException">If <typeparamref name="TDependency"/> is not a interface</exception>
        /// <returns></returns>
        public StronglyTypedDecoratorBinding<TDependency> Decorate<TDependency>()
        {
            var decorator = new StronglyTypedDecoratorBinding<TDependency>();
            StronglyTypedBinding<TDependency> binding = GetOrCreateBinding<TDependency>();
            binding.Decorators.Add(decorator);
            return decorator;
        }

        public void AddBinding(IBinding binding)
        {
            _bindings.Add(binding.DependencyType, binding);
        }

		public IEnumerator<IBinding> GetEnumerator()
		{
			return Bindings.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private StronglyTypedBinding<TDependency> GetOrCreateBinding<TDependency>()
		{
			if (Bindings.TryGetValue(typeof(TDependency), out IBinding weaklyTypedBinding))
			{
				return (StronglyTypedBinding<TDependency>)weaklyTypedBinding;
			}
			var binding = new StronglyTypedBinding<TDependency>();
			AddBinding(binding);
			return binding;
		}
	}
}