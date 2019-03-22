using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Singularity.Enums;
using Singularity.Exceptions;
using Singularity.Graph;

namespace Singularity.Bindings
{
    /// <summary>
    /// Represents a strongly typed binding registration for <typeparamref name="TDependency"/>.
    /// </summary>
    /// <typeparam name="TDependency"></typeparam>
	public sealed class StronglyTypedBinding<TDependency> : IBinding
	{
        /// <summary>
        /// The metadata of this binding.
        /// </summary>
		public BindingMetadata BindingMetadata { get; }

        /// <summary>
        /// The decorators for this binding.
        /// </summary>
		public List<IDecoratorBinding> Decorators { get; } = new List<IDecoratorBinding>();
	    IReadOnlyList<IDecoratorBinding> IBinding.Decorators => Decorators;

        /// <summary>
        /// The type of this binding.
        /// </summary>
		public Type DependencyType { get; } = typeof(TDependency);

        /// <summary>
        /// A expression that is used to generate a instance.
        /// </summary>
		public Expression? Expression => ConfiguredBinding?.Expression;

        /// <summary>
        /// The lifetime of the generated instance(s).
        /// </summary>
		public Lifetime Lifetime => ConfiguredBinding?.Lifetime ?? Lifetime.PerCall;

        /// <summary>
        /// A action that will be invoked on the generated instance(s) when the container in which this binding is used is disposed.
        /// </summary>
        public Action<object>? OnDeath => ConfiguredBinding?.OnDeath;
	    private IConfiguredBinding? ConfiguredBinding { get; set; }

        internal StronglyTypedBinding(string callerFilePath, int callerLineNumber, IModule? module)
		{
			BindingMetadata = new BindingMetadata(callerFilePath, callerLineNumber, module);
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

        /// <summary>
        /// <see cref="Inject{TInstance}(Expression{Func{TInstance}})"/>
        /// </summary>
		public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1>(Expression<Func<TP1, TInstance>> expression)
			where TInstance : class, TDependency
		{
			return SetExpression<TInstance>(expression);
		}

        /// <summary>
        /// <see cref="Inject{TInstance}(Expression{Func{TInstance}})"/>
        /// </summary>
        public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1, TP2>(Expression<Func<TP1, TP2, TInstance>> expression)
			where TInstance : class, TDependency
		{
			return SetExpression<TInstance>(expression);
		}

        /// <summary>
        /// <see cref="Inject{TInstance}(Expression{Func{TInstance}})"/>
        /// </summary>
        public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1, TP2, TP3>(Expression<Func<TP1, TP2, TP3, TInstance>> expression)
			where TInstance : class, TDependency
		{
			return SetExpression<TInstance>(expression);
		}

        /// <summary>
        /// <see cref="Inject{TInstance}(Expression{Func{TInstance}})"/>
        /// </summary>
        public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1, TP2, TP3, TP4>(Expression<Func<TP1, TP2, TP3, TP4, TInstance>> expression)
			where TInstance : class, TDependency
		{
			return SetExpression<TInstance>(expression);
		}

        /// <summary>
        /// <see cref="Inject{TInstance}(Expression{Func{TInstance}})"/>
        /// </summary>
        public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1, TP2, TP3, TP4, TP5>(Expression<Func<TP1, TP2, TP3, TP4, TP5, TInstance>> expression)
			where TInstance : class, TDependency
		{
			return SetExpression<TInstance>(expression);
		}

        /// <summary>
        /// <see cref="Inject{TInstance}(Expression{Func{TInstance}})"/>
        /// </summary>
        public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1, TP2, TP3, TP4, TP5, TP6>(Expression<Func<TP1, TP2, TP3, TP4, TP5, TP6, TInstance>> expression)
			where TInstance : class, TDependency
		{
			return SetExpression<TInstance>(expression);
		}

        /// <summary>
        /// <see cref="Inject{TInstance}(Expression{Func{TInstance}})"/>
        /// </summary>
        public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1, TP2, TP3, TP4, TP5, TP6, TP7>(Expression<Func<TP1, TP2, TP3, TP4, TP5, TP6, TP7, TInstance>> expression)
			where TInstance : class, TDependency
		{
			return SetExpression<TInstance>(expression);
		}

        /// <summary>
        /// <see cref="Inject{TInstance}(Expression{Func{TInstance}})"/>
        /// </summary>
        public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8>(Expression<Func<TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TInstance>> expression)
			where TInstance : class, TDependency
		{
			return SetExpression<TInstance>(expression);
		}

		internal StronglyTypedConfiguredBinding<TDependency, TInstance> SetExpression<TInstance>(Expression expression) 
            where TInstance : class
		{
			var configuredBinding = new StronglyTypedConfiguredBinding<TDependency, TInstance>(expression);
			ConfiguredBinding = configuredBinding;
			return configuredBinding;
		}
	}
}
