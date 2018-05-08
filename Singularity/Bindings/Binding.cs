using System;
using System.Linq.Expressions;

namespace Singularity
{
    public class Binding<TDependency> : IBinding
    {
        public Type DependencyType { get; } = typeof(TDependency);
        public Expression Expression { get; set; }
        public Lifetime Lifetime { get; private set; }

        /// <summary>
        /// Sets the actual type that will be used for the dependency and auto generates a <see cref="System.Linq.Expressions.Expression"/> to call the constructor
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <returns></returns>
        public Binding<TDependency> To<TInstance>()
            where TInstance : TDependency
        {
            return To(typeof(TInstance));
        }

        public Binding<TDependency> To(Type type)
        {
            return SetExpression(type.AutoResolveConstructor());
        }

        private Binding<TDependency> To(Expression expression)
        {
            return SetExpression(expression);
        }

        public Binding<TDependency> To<TInstance>(Expression<Func<TInstance>> expression)
            where TInstance : TDependency
        {
            return SetExpression(expression);
        }

        public Binding<TDependency> To<TInstance, TP1>(Expression<Func<TP1, TInstance>> expression)
            where TInstance : TDependency
        {
            return SetExpression(expression);
        }

        private Binding<TDependency> SetExpression(Expression expression)
        {
            Expression = expression;
            return this;
        }

        public Binding<TDependency> SetLifetime(Lifetime lifetime)
        {
            Lifetime = lifetime;
            return this;
        }
    }
}
