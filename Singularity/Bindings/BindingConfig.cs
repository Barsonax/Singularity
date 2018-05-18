using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Singularity.Graph;

namespace Singularity.Bindings
{
	public interface IBindingConfig
	{
		IReadOnlyDictionary<Type, IBinding> Bindings { get; }
	}

	public class ReadOnlyBindingConfig : IBindingConfig
	{
		public IReadOnlyDictionary<Type, IBinding> Bindings { get; }

		public ReadOnlyBindingConfig(IBindingConfig bindingConfig)
		{
			Bindings = bindingConfig.Bindings;
		}
	}

	public class BindingConfig : IBindingConfig
	{
		private readonly Dictionary<Type, IBinding> _bindings = new Dictionary<Type, IBinding>();

		public IReadOnlyDictionary<Type, IBinding> Bindings => _bindings;

		public BindingConfig()
		{

		}

		internal BindingConfig(IBindingConfig childBindingConfig, DependencyGraph parentDependencyGraph)
		{
			foreach (var binding in childBindingConfig.Bindings.Values)
			{
				_bindings.Add(binding.DependencyType, binding);
			}

			foreach (var parentBinding in parentDependencyGraph._bindingConfig.Bindings.Values)
			{
				if (_bindings.TryGetValue(parentBinding.DependencyType, out var binding))
				{
					if (binding.ConfiguredBinding != null) continue;				
					var oldConfiguredBinding = binding.ConfiguredBinding ?? parentBinding.ConfiguredBinding;

					List<IDecoratorBinding> decorators;
					Expression expression;
					Action<object> onDeathAction;
					if (oldConfiguredBinding.Lifetime == Lifetime.PerContainer)
					{
						expression = parentDependencyGraph.Dependencies[binding.DependencyType].ResolvedDependency.Expression;
						decorators = binding.Decorators.ToList();
						onDeathAction = null;
					}
					else
					{
						onDeathAction = oldConfiguredBinding.OnDeath;
						decorators = parentBinding.Decorators.Concat(binding.Decorators).ToList();
						expression = oldConfiguredBinding.Expression;
					}
					var weaklyTypedConfiguredBinding = new WeaklyTypedConfiguredBinding(expression, oldConfiguredBinding.Lifetime, onDeathAction);
					var weaklyTypedBinding = new WeaklyTypedBinding(binding.DependencyType, weaklyTypedConfiguredBinding, decorators);
					_bindings[binding.DependencyType] = weaklyTypedBinding;
				}
				else
				{
					if (parentBinding.ConfiguredBinding?.Lifetime == Lifetime.PerContainer)
					{
						var weaklyTypedConfiguredBinding = new WeaklyTypedConfiguredBinding(
							parentBinding.ConfiguredBinding.Expression, parentBinding.ConfiguredBinding.Lifetime, null);
						var weaklyTypedBinding =
							new WeaklyTypedBinding(parentBinding.DependencyType, weaklyTypedConfiguredBinding, parentBinding.Decorators);
						_bindings.Add(parentBinding.DependencyType, weaklyTypedBinding);
					}
					else
					{
						_bindings.Add(parentBinding.DependencyType, parentBinding);
					}
				}
			}
		}

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
			var binding = GetOrCreateBinding<TDependency>();
			binding.Decorators.Add(decorator);
			return decorator;
		}

		private StronglyTypedBinding<TDependency> GetOrCreateBinding<TDependency>()
		{
			if (Bindings.TryGetValue(typeof(TDependency), out var weaklyTypedBinding))
			{
				return (StronglyTypedBinding<TDependency>)weaklyTypedBinding;
			}
			var binding = new StronglyTypedBinding<TDependency>();
			AddBinding(binding);
			return binding;
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