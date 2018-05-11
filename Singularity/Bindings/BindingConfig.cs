using System;
using System.Collections.Generic;
using System.Linq;
using Singularity.Bindings;

namespace Singularity
{
    public interface IBindingConfig
    {
        IReadOnlyDictionary<Type, IBinding> Bindings { get; }
        IEnumerable<IDecoratorBinding> Decorators { get; }
    }

    public class ReadOnlyBindingConfig : IBindingConfig
    {
        public IReadOnlyDictionary<Type, IBinding> Bindings { get; }

        public IEnumerable<IDecoratorBinding> Decorators { get; }        

        public ReadOnlyBindingConfig(IBindingConfig bindingConfig)
        {
            Bindings = bindingConfig.Bindings;
            Decorators = bindingConfig.Decorators.ToArray();
        }
    }

    public class BindingConfig : IBindingConfig
    {
        private readonly Dictionary<Type, IBinding> _bindings = new Dictionary<Type, IBinding>();
        private readonly List<IDecoratorBinding> _decorators = new List<IDecoratorBinding>();

        public IReadOnlyDictionary<Type, IBinding> Bindings => _bindings;

        public IEnumerable<IDecoratorBinding> Decorators => _decorators;

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
			_decorators.Add(decorator);
            return decorator;
        }

        public void AddBinding(IBinding binding)
        {            
            _bindings.Add(binding.DependencyType, binding);
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