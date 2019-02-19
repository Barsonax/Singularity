using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Singularity.Enums;
using Singularity.Exceptions;
using Singularity.Graph;

namespace Singularity.Bindings
{
	public sealed class StronglyTypedBinding<TDependency> : IBinding
	{
		public BindingMetadata BindingMetadata { get; }
		public List<IDecoratorBinding> Decorators { get; } = new List<IDecoratorBinding>();
	    IReadOnlyList<IDecoratorBinding> IBinding.Decorators => Decorators;
		public Type DependencyType { get; } = typeof(TDependency);

		public Expression? Expression => ConfiguredBinding?.Expression;
		public Lifetime Lifetime => ConfiguredBinding?.Lifetime ?? Lifetime.PerCall;
        public Action<object>? OnDeath => ConfiguredBinding?.OnDeath;
	    private IConfiguredBinding? ConfiguredBinding { get; set; }

        internal StronglyTypedBinding(string callerFilePath, int callerLineNumber, IModule? module)
		{
			BindingMetadata = new BindingMetadata(callerFilePath, callerLineNumber, module);
		}

        /// <summary>
        /// Sets the actual type that will be used for the dependency and auto generates a <see cref="System.Linq.Expressions.Expression"/> to call the constructor.
        /// This generated expression will never return null.
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <exception cref="NoConstructorException">If there is no public constructor</exception>
        /// <exception cref="CannotAutoResolveConstructorException">If there is more than 1 public constructors</exception>
        /// <returns></returns>
        /// <overloads></overloads>
        public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance>()
			where TInstance : class, TDependency
		{
			return SetExpression<TInstance>(typeof(TInstance).AutoResolveConstructorExpression());
		}

		/// <summary>
		/// Lets you provide a expression <see cref="System.Linq.Expressions.Expression"/> to create the instance constructor.
		/// Be careful as there will be no exception if this expression returns a null instance.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <returns></returns>
		public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance>(Expression<Func<TInstance>> expression)
			where TInstance : class, TDependency
		{
			return SetExpression<TInstance>(expression);
		}

		public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1>(Expression<Func<TP1, TInstance>> expression)
			where TInstance : class, TDependency
		{
			return SetExpression<TInstance>(expression);
		}

		public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1, TP2>(Expression<Func<TP1, TP2, TInstance>> expression)
			where TInstance : class, TDependency
		{
			return SetExpression<TInstance>(expression);
		}

		public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1, TP2, TP3>(Expression<Func<TP1, TP2, TP3, TInstance>> expression)
			where TInstance : class, TDependency
		{
			return SetExpression<TInstance>(expression);
		}

		public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1, TP2, TP3, TP4>(Expression<Func<TP1, TP2, TP3, TP4, TInstance>> expression)
			where TInstance : class, TDependency
		{
			return SetExpression<TInstance>(expression);
		}

		public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1, TP2, TP3, TP4, TP5>(Expression<Func<TP1, TP2, TP3, TP4, TP5, TInstance>> expression)
			where TInstance : class, TDependency
		{
			return SetExpression<TInstance>(expression);
		}

		public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1, TP2, TP3, TP4, TP5, TP6>(Expression<Func<TP1, TP2, TP3, TP4, TP5, TP6, TInstance>> expression)
			where TInstance : class, TDependency
		{
			return SetExpression<TInstance>(expression);
		}

		public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1, TP2, TP3, TP4, TP5, TP6, TP7>(Expression<Func<TP1, TP2, TP3, TP4, TP5, TP6, TP7, TInstance>> expression)
			where TInstance : class, TDependency
		{
			return SetExpression<TInstance>(expression);
		}

		public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8>(Expression<Func<TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TInstance>> expression)
			where TInstance : class, TDependency
		{
			return SetExpression<TInstance>(expression);
		}

		private StronglyTypedConfiguredBinding<TDependency, TInstance> SetExpression<TInstance>(Expression expression) where TInstance : class
		{
			var configuredBinding = new StronglyTypedConfiguredBinding<TDependency, TInstance>(expression);
			ConfiguredBinding = configuredBinding;
			return configuredBinding;
		}
	}
}
