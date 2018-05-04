using System;
using System.Linq.Expressions;

namespace Singularity
{
    public class Binding<TDependency> : IBinding
    {
        public Type DependencyType { get; } = typeof(TDependency);
        public Expression Expression { get; set; }
        public Lifetime Lifetime { get; private set; }

        public Binding<TDependency> To<TInstance>()
            where TInstance : TDependency
        {
            Expression = typeof(TInstance).AutoResolveConstructor();
            return this;
        }

        public Binding<TDependency> To(Type type)
        {
            Expression = type.AutoResolveConstructor();
            return this;
        }

        public Binding<TDependency> To<TInstance>(Expression<Func<TInstance>> expression)
            where TInstance : TDependency
        {
            Expression = expression;
            return this;
        }

        public Binding<TDependency> To<TInstance, TP1>(Expression<Func<TP1, TInstance>> expression)
            where TInstance : TDependency
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
