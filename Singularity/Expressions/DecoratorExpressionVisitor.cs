using System;
using System.Linq;
using System.Linq.Expressions;
using Singularity.Graph;

namespace Singularity.Expressions
{
    internal sealed class DecoratorExpressionVisitor : ExpressionVisitor
    {
        private readonly InstanceFactory[] _factories;
        private readonly Type _instanceType;
        public Expression? PreviousDecorator;

        public DecoratorExpressionVisitor(InstanceFactory[] factories, Type instanceType)
        {
            _factories = factories;
            _instanceType = instanceType;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node.Type == _instanceType)
            {
                return PreviousDecorator ?? throw new InvalidOperationException($"You should assign {nameof(PreviousDecorator)} first");
            }
            else
            {
                InstanceFactory factory = _factories.First(x => x.DependencyType == node.Type);
                return factory.Expression!;
            }
        }
    }
}