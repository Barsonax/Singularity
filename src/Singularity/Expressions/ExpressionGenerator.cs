using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.FastExpressionCompiler;
using Singularity.Graph;

namespace Singularity.Expressions
{
    internal sealed class ExpressionGenerator
    {
        public static readonly ParameterExpression ScopeParameter = Expression.Parameter(typeof(Scoped));


        public ReadOnlyExpressionContext GenerateBaseExpression(ServiceBinding serviceBinding, InstanceFactory[] children, Scoped containerScope, SingularitySettings settings)
        {
            var context = new ExpressionContext(serviceBinding.Expression);
            if (serviceBinding.Expression is AbstractBindingExpression)
            {
                context.Expression = Expression.Constant(serviceBinding);
                return context;
            }
            context.Expression = serviceBinding.Expression! is LambdaExpression lambdaExpression ? lambdaExpression.Body : serviceBinding.Expression;
            var parameterExpressionVisitor = new ParameterExpressionVisitor(context, children!);
            context.Expression = parameterExpressionVisitor.Visit(context.Expression);

            if (ShouldBeAutoDisposed(serviceBinding, context, settings))
            {
                MethodInfo method = Scoped.AddDisposableMethod.MakeGenericMethod(context.Expression.Type);
                MethodCallExpression methodCallExpression = Expression.Call(ScopeParameter, method, context.Expression);
                context.Expression = methodCallExpression;
            }

            if (serviceBinding.Finalizer != null)
            {
                MethodInfo method = Scoped.AddFinalizerMethod.MakeGenericMethod(context.Expression.Type);
                context.Expression = Expression.Call(ScopeParameter, method, context.Expression, Expression.Constant(serviceBinding));
            }

            serviceBinding.Lifetime.ApplyCaching(containerScope, context);
            return context;
        }

        private bool ShouldBeAutoDisposed(ServiceBinding serviceBinding, ExpressionContext context, SingularitySettings settings)
        {
            return serviceBinding.NeedsDispose == ServiceAutoDispose.Always ||
                   serviceBinding.NeedsDispose != ServiceAutoDispose.Never && typeof(IDisposable).IsAssignableFrom(context.Expression.Type) && settings.AutoDispose.Contains(serviceBinding.Lifetime);
 }

        public ReadOnlyExpressionContext ApplyDecorators(Type dependencyType, ServiceBinding serviceBinding, InstanceFactory[] children, Expression[] decorators, Scoped containerScope)
        {
            ExpressionContext context = (ExpressionContext)(serviceBinding.BaseExpression ?? throw new ArgumentNullException($"{nameof(serviceBinding)}.{nameof(serviceBinding.BaseExpression)}"));
            if (decorators.Length > 0)
            {
                var body = new List<Expression>();
                ParameterExpression instanceParameter = Expression.Variable(dependencyType, $"{dependencyType} instance");
                body.Add(Expression.Assign(instanceParameter, Expression.Convert(context.Expression, dependencyType)));

                var decoratorExpressionVisitor = new DecoratorExpressionVisitor(children!, instanceParameter.Type);
                decoratorExpressionVisitor.PreviousDecorator = instanceParameter;
                foreach (Expression decorator in decorators)
                {
                    Expression decoratorExpression = decorator;

                    decoratorExpression = decoratorExpressionVisitor.Visit(decoratorExpression);

                    decoratorExpressionVisitor.PreviousDecorator = decoratorExpression;
                }

                body.Add(decoratorExpressionVisitor.PreviousDecorator);

                if (body.Last().Type == typeof(void)) body.Add(instanceParameter);
                context.Expression = body.Count == 1 ? context.Expression : Expression.Block(new[] { instanceParameter }, body);
                serviceBinding.Lifetime.ApplyCaching(containerScope, context);
            }

            return context;
        }
    }
}
