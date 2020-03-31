using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Exceptions;

namespace Singularity
{
    internal static class DecoratorTypeValidator
    {
        public static void CheckIsInterface(Type serviceType)
        {
            if (!serviceType.GetTypeInfo().IsInterface) throw new InterfaceExpectedException($"{serviceType} is not a interface.");
        }

        public static void CheckParameters(Expression expression, Type serviceType, Type decoratorType)
        {
            ParameterExpression[] parameters = expression.GetParameterExpressions();
            if (parameters.All(x => x.Type != serviceType)) throw new InvalidExpressionArgumentsException($"Cannot decorate {serviceType} since the expression to create {decoratorType} does not have a parameter for {serviceType}");
        }
    }
}
