using System;
using System.Linq.Expressions;

namespace Singularity.Resolving.Generators
{
    /// <summary>
    /// Creates bindings so that the expression itself of a binding can be resolved
    /// </summary>
    public sealed class ExpressionServiceBindingGenerator : IGenericWrapperGenerator
    {
        public bool CanResolve(Type type)
        {
            if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Expression<>) && type.GenericTypeArguments.Length == 1)
            {
                Type funcType = type.GenericTypeArguments[0];
                return funcType.GetGenericTypeDefinition() == typeof(Func<>) && funcType.GenericTypeArguments.Length == 1;
            }
            return false;
        }

        public Type DependsOn(Type type) => typeof(Func<>).MakeGenericType(type);

        public Type Target(Type type)
        {
            var funcType = typeof(Func<>).MakeGenericType(type);
            return typeof(Expression<>).MakeGenericType(funcType);
        }

        public Expression Wrap(Expression expression, Type unWrappedType, Type wrappedType)
        {
            return Expression.Constant(((LambdaExpression)expression).Body, wrappedType);
        }
    }
}