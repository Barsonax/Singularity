using System;
using System.Collections.Generic;
using Singularity.Bindings;

namespace Singularity
{
    public class BindingConfig
    {
        public readonly Dictionary<Type, IBinding> Bindings = new Dictionary<Type, IBinding>();
        public readonly List<IDecoratorBinding> Decorators = new List<IDecoratorBinding>();

        /// <summary>
        /// Begins configuring a binding for a certain dependency
        /// </summary>
        /// <typeparam name="TDependency"></typeparam>
        /// <returns></returns>
        public StronglyTypedBinding<TDependency> For<TDependency>()
        {
            var binding = new StronglyTypedBinding<TDependency>();
            AddBinding(binding);
            return binding;
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
			Decorators.Add(decorator);
            return decorator;
        }

        public void AddBinding(IBinding binding)
        {            
            Bindings.Add(binding.DependencyType, binding);
        }

        public void ValidateBindings()
        {
            foreach (var binding in Bindings.Values)
            {
                if (binding.ConfiguredBinding?.Expression == null) throw new NullReferenceException();
            }
        }
    }
}