using System;
using System.Linq;
using System.Linq.Expressions;
using Singularity.Graph;

namespace Singularity.Expressions
{
    internal sealed class DecoratorExpressionVisitor : ExpressionVisitor
    {
        private readonly Dependency[] _dependencies;
        private Type _instanceType;
        public Expression PreviousDecorator;

        public DecoratorExpressionVisitor(Dependency[] dependencies, Type instanceType)
        {
            _dependencies = dependencies;
            _instanceType = instanceType;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node.Type == _instanceType)
            {
                return PreviousDecorator;
            }
            else
            {
                var decoratorDependency = _dependencies.First(x => x.Registration.DependencyType == node.Type);
                return decoratorDependency.Default.Expression;
            }
        }
    }
}