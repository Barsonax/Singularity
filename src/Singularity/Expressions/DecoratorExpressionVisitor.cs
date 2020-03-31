using System;
using System.Linq;
using System.Linq.Expressions;

using Singularity.Resolving;

namespace Singularity.Expressions
{
    internal sealed class DecoratorExpressionVisitor : ExpressionVisitor
    {
        private readonly InstanceFactory[] _factories;
        private readonly Type _serviceType;
        public Expression? PreviousDecorator;

        public DecoratorExpressionVisitor(InstanceFactory[] factories, Type serviceType)
        {
            _factories = factories;
            _serviceType = serviceType;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node.Type == _serviceType)
            {
                return PreviousDecorator ?? throw new InvalidOperationException($"You should assign {nameof(PreviousDecorator)} first");
            }
            else
            {
                InstanceFactory factory = _factories.First(x => x.ServiceType == node.Type);
                return factory.Context.Expression;
            }
        }
    }
}