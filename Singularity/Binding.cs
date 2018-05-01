using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity
{
    public class Binding<TDependency> : IBinding
    {
        public Type DependencyType { get; } = typeof(TDependency);
        public Expression BindingExpression { get; set; }
        public Lifetime Lifetime { get; private set; }
        public bool IsResolved { get; set; }

        public Binding<TDependency> To<TInstance>()
            where TInstance : TDependency
        {
            BindingExpression = AutoResolveConstructor(typeof(TInstance));
            return this;
        }

        private Expression AutoResolveConstructor(Type type)
        {
            var constructors = type.GetTypeInfo().DeclaredConstructors.Where(x => x.IsPublic).ToArray();
            if (constructors.Length == 0) throw new NoConstructorException($"Type {type} did not contain any public constructor.");
            if (constructors.Length > 1) throw new CannotAutoResolveConstructorException($"Found {constructors.Length} suitable constructors for type {type}. Please specify the constructor explicitly.");
            var constructor = constructors.First();
            var parameters = constructor.GetParameters();
            var parameterExpressions = new Expression[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                parameterExpressions[i] = Expression.Parameter(parameters[i].ParameterType);
            }
            return Expression.New(constructor, parameterExpressions);
        }

        public Binding<TDependency> To<TInstance>(Expression<Func<TInstance>> expression)
            where TInstance : TDependency
        {
            BindingExpression = expression;
            return this;
        }

        public Binding<TDependency> To<TInstance, TP1>(Expression<Func<TP1, TInstance>> expression)
            where TInstance : TDependency
        {
            BindingExpression = expression;
            return this;
        }

        public Binding<TDependency> SetLifetime(Lifetime lifetime)
        {
            Lifetime = lifetime;
            return this;
        }
    }
}
