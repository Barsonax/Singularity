using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity.Bindings
{
    /// <summary>
    /// Represents a strongly typed binding registration for <typeparamref name="TDependency"/>.
    /// </summary>
    /// <typeparam name="TDependency"></typeparam>
	public sealed class StronglyTypedBinding<TDependency> : WeaklyTypedBinding
    {
        internal StronglyTypedBinding(string callerFilePath, int callerLineNumber, IModule? module) : base(typeof(TDependency), callerFilePath, callerLineNumber, module)
        {

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
            return Inject<TInstance>((Expression)expression);
        }

        /// <summary>
        /// <see cref="Inject{TInstance}(Expression{Func{TInstance}})"/>
        /// </summary>
		public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1>(Expression<Func<TP1, TInstance>> expression)
            where TInstance : class, TDependency
        {
            return Inject<TInstance>(expression);
        }

        /// <summary>
        /// <see cref="Inject{TInstance}(Expression{Func{TInstance}})"/>
        /// </summary>
        public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1, TP2>(Expression<Func<TP1, TP2, TInstance>> expression)
            where TInstance : class, TDependency
        {
            return Inject<TInstance>(expression);
        }

        /// <summary>
        /// <see cref="Inject{TInstance}(Expression{Func{TInstance}})"/>
        /// </summary>
        public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1, TP2, TP3>(Expression<Func<TP1, TP2, TP3, TInstance>> expression)
            where TInstance : class, TDependency
        {
            return Inject<TInstance>(expression);
        }

        /// <summary>
        /// <see cref="Inject{TInstance}(Expression{Func{TInstance}})"/>
        /// </summary>
        public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1, TP2, TP3, TP4>(Expression<Func<TP1, TP2, TP3, TP4, TInstance>> expression)
            where TInstance : class, TDependency
        {
            return Inject<TInstance>(expression);
        }

        /// <summary>
        /// <see cref="Inject{TInstance}(Expression{Func{TInstance}})"/>
        /// </summary>
        public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1, TP2, TP3, TP4, TP5>(Expression<Func<TP1, TP2, TP3, TP4, TP5, TInstance>> expression)
            where TInstance : class, TDependency
        {
            return Inject<TInstance>(expression);
        }

        /// <summary>
        /// <see cref="Inject{TInstance}(Expression{Func{TInstance}})"/>
        /// </summary>
        public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1, TP2, TP3, TP4, TP5, TP6>(Expression<Func<TP1, TP2, TP3, TP4, TP5, TP6, TInstance>> expression)
            where TInstance : class, TDependency
        {
            return Inject<TInstance>(expression);
        }

        /// <summary>
        /// <see cref="Inject{TInstance}(Expression{Func{TInstance}})"/>
        /// </summary>
        public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1, TP2, TP3, TP4, TP5, TP6, TP7>(Expression<Func<TP1, TP2, TP3, TP4, TP5, TP6, TP7, TInstance>> expression)
            where TInstance : class, TDependency
        {
            return Inject<TInstance>(expression);
        }

        /// <summary>
        /// <see cref="Inject{TInstance}(Expression{Func{TInstance}})"/>
        /// </summary>
        public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8>(Expression<Func<TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TInstance>> expression)
            where TInstance : class, TDependency
        {
            return Inject<TInstance>(expression);
        }

        internal StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance>(Expression expression)
            where TInstance : class, TDependency
        {
            var configuredBinding = new StronglyTypedConfiguredBinding<TDependency, TInstance>(this ,expression);
            WeaklyTypedConfiguredBinding = configuredBinding;
            return configuredBinding;
        }
    }
}
