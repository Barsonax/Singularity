using System;
using System.Linq.Expressions;

namespace Singularity.Bindings
{
    public class StronglyTypedConfiguredBinding<TDependency, TInstance> : IConfiguredBinding
        where TInstance : class
    {
        public Expression Expression { get; }
        public Lifetime Lifetime { get; private set; }
        public Action<TInstance> OnDeathAction { get; private set; }
        Action<object> IConfiguredBinding.OnDeath => OnDeathAction != null ? (Action<object>) (obj => OnDeathAction((TInstance) obj)) : null;

        public StronglyTypedConfiguredBinding(Expression expression)
        {
            Expression = expression;
        }

        public StronglyTypedConfiguredBinding<TDependency, TInstance> With(Lifetime lifetime)
        {
            Lifetime = lifetime;
            return this;
        }

        public StronglyTypedConfiguredBinding<TDependency, TInstance> OnDeath(Action<TInstance> onDeathAction)
        {
            OnDeathAction = onDeathAction;
            return this;
        }
    }

    public class StronglyTypedBinding<TDependency> : IBinding
    {
        public IConfiguredBinding ConfiguredBinding { get; private set; }
        public Type DependencyType { get; } = typeof(TDependency);

        /// <summary>
        /// Sets the actual type that will be used for the dependency and auto generates a <see cref="System.Linq.Expressions.Expression"/> to call the constructor
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <returns></returns>
        public StronglyTypedConfiguredBinding<TDependency, TInstance> Inject<TInstance>()
            where TInstance : class, TDependency
        {
            return SetExpression<TInstance>(typeof(TInstance).AutoResolveConstructor());
        }

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
