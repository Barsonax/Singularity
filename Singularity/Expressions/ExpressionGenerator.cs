using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FastExpressionCompiler;
using Singularity.Graph;

namespace Singularity.Expressions
{
    internal class ExpressionGenerator
    {
        public static ParameterExpression ScopeParameter = Expression.Parameter(typeof(Scoped));

        public Expression GenerateDependencyExpression(ResolvedDependency dependency, Scoped containerScope)
        {
            Expression expression = dependency.Binding.Expression! is LambdaExpression lambdaExpression ? lambdaExpression.Body : dependency.Binding.Expression;
            var parameterExpressionVisitor = new ParameterExpressionVisitor(dependency.Children);
            expression = parameterExpressionVisitor.Visit(expression);

            if (dependency.Binding.OnDeathAction != null)
            {
                MethodInfo method = Scoped.GenericAddMethod.MakeGenericMethod(expression.Type);
                expression = Expression.Call(ScopeParameter, method, expression, Expression.Constant(dependency.Binding));
            }

            if (dependency.Registration.Decorators.Count > 0)
            {
                var body = new List<Expression>();
                ParameterExpression instanceParameter = Expression.Variable(dependency.Registration.DependencyType, $"{expression.Type} instance");
                body.Add(Expression.Assign(instanceParameter, Expression.Convert(expression, dependency.Registration.DependencyType)));

                if (dependency.Registration.Decorators.Count > 0)
                {
                    var decoratorExpressionVisitor = new DecoratorExpressionVisitor(dependency.Children, instanceParameter.Type);
                    decoratorExpressionVisitor.PreviousDecorator = instanceParameter;
                    foreach (Expression decorator in dependency.Registration.Decorators)
                    {
                        Expression decoratorExpression = decorator;

                        decoratorExpression = decoratorExpressionVisitor.Visit(decoratorExpression);

                        decoratorExpressionVisitor.PreviousDecorator = decoratorExpression;
                    }
                    body.Add(decoratorExpressionVisitor.PreviousDecorator);
                }

                if (body.Last().Type == typeof(void)) body.Add(instanceParameter);
                expression = body.Count == 1 ? expression : Expression.Block(new[] { instanceParameter }, body);
            }

            switch (dependency.Binding.Lifetime)
            {
                case Lifetime.Transient:
                    return expression;
                case Lifetime.PerContainer:
                    object singletonInstance = ((Func<Scoped, object>)Expression.Lambda(expression, ScopeParameter).CompileFast())(containerScope);
                    dependency.InstanceFactory = scope => singletonInstance;
                    return Expression.Constant(singletonInstance, dependency.Registration.DependencyType);
                case Lifetime.PerScope:
                    var scopedFactory = (Func<Scoped, object>)Expression.Lambda(expression, ScopeParameter).CompileFast();
                    expression = Expression.Call(ScopeParameter, Scoped.GetorAddScopedInstanceMethod, Expression.Constant(dependency.Registration.DependencyType), Expression.Constant(scopedFactory));
                    return expression;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
