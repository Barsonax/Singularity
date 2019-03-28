using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Singularity.Exceptions;

namespace Singularity.Bindings
{
    public sealed class StronglyTypedDecoratorBinding<TDependency> : WeaklyTypedDecoratorBinding
        where TDependency : class
    {
        internal StronglyTypedDecoratorBinding() : base(typeof(TDependency))
        {

        }

        /// <summary>
        /// Sets the type which should be wrapped by the decorator
        /// </summary>
        /// <typeparam name="TDecorator"></typeparam>
        /// <returns></returns>
        public StronglyTypedDecoratorBinding<TDependency> With<TDecorator>()
            where TDecorator : TDependency
        {
            TypeInfo typeInfo = typeof(TDecorator).GetTypeInfo();
            if (!typeof(TDependency).GetTypeInfo().IsAssignableFrom(typeInfo)) throw new InterfaceNotImplementedException($"{typeof(TDependency)} is not implemented by {typeof(TDecorator)}");

            Expression = AutoResolveConstructorExpressionCache<TDecorator>.Expression;
            ParameterExpression[] parameters = Expression.GetParameterExpressions();
            if (parameters.All(x => x.Type != typeof(TDependency))) throw new InvalidExpressionArgumentsException($"Cannot decorate {typeof(TDependency)} since the expression to create {typeof(TDecorator)} does not have a parameter for {typeof(TDependency)}");

            return this;
        }

        public override WeaklyTypedDecoratorBinding With(Type decoratorType)
        {
            MethodInfo withMethod = (from m in typeof(StronglyTypedDecoratorBinding<TDependency>).GetRuntimeMethods()
                where m.Name == nameof(With)
                where m.IsGenericMethod
                where m.GetGenericArguments().Length == 1
                select m).First().MakeGenericMethod(decoratorType);

            return (WeaklyTypedDecoratorBinding)withMethod.Invoke(this, new object[0]);
        }
    }
}