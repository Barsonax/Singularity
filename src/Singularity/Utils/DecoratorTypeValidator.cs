using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Exceptions;

namespace Singularity
{
    internal static class DecoratorTypeValidator
    {
        public static void CheckIsInterface(Type dependencyType)
        {
            if (!dependencyType.GetTypeInfo().IsInterface) throw new InterfaceExpectedException($"{dependencyType} is not a interface.");
        }

        public static void CheckParameters(Expression expression, Type dependencyType, Type decoratorType)
        {
            ParameterExpression[] parameters = expression.GetParameterExpressions();
            if (parameters.All(x => x.Type != dependencyType)) throw new InvalidExpressionArgumentsException($"Cannot decorate {dependencyType} since the expression to create {decoratorType} does not have a parameter for {dependencyType}");
        }
    }
}
