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
        private static readonly MethodInfo AddMethod = typeof(ObjectActionList).GetRuntimeMethod(nameof(ObjectActionList.Add), new[] { typeof(object) });

        public Expression GenerateDependencyExpression(Dependency dependency, Scoped scope)
        {
            Expression expression = dependency.Binding.Expression! is LambdaExpression lambdaExpression ? lambdaExpression.Body : dependency.Binding.Expression;
            var parameterExpressionVisitor = new ParameterExpressionVisitor(dependency.Dependencies);
            expression = parameterExpressionVisitor.Visit(expression);

            if (dependency.Binding.OnDeathAction != null || dependency.Binding.Decorators.Length > 0)
            {
                var body = new List<Expression>();
                ParameterExpression instanceParameter = Expression.Variable(dependency.Binding.DependencyType, $"{expression.Type} instance");
                body.Add(Expression.Assign(instanceParameter, Expression.Convert(expression, dependency.Binding.DependencyType)));

                if (dependency.Binding.OnDeathAction != null)
                {
                    ObjectActionList actionList = scope.GetActionList(dependency.Binding.Expression.Type, dependency.Binding.OnDeathAction);
                    body.Add(Expression.Call(Expression.Constant(actionList), AddMethod, instanceParameter));
                }

                if (dependency.Binding.Decorators.Length > 0)
                {
                    var decoratorExpressionVisitor = new DecoratorExpressionVisitor(dependency.Dependencies, instanceParameter.Type);
                    decoratorExpressionVisitor.PreviousDecorator = instanceParameter;
                    foreach (Expression decorator in dependency.Binding.Decorators)
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

            if (dependency.Binding.CreationMode is CreationMode.Singleton)
            {
                object value;
                if (expression is NewExpression newExpression && newExpression.Arguments.Count == 0)
                {
                    //In this case we know the signature and can call the constructor directly instead of doing a costly compile.
                    value = newExpression.Constructor.Invoke(null);
                }
                else
                {
                    value = ((Func<object>)Expression.Lambda(expression).CompileFast()).Invoke();
                }
                dependency.InstanceFactory = () => value;
                return Expression.Constant(value, dependency.Binding.DependencyType);
            }
            return expression;
        }
    }
}
