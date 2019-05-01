using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.FastExpressionCompiler;
using Singularity.Graph;

namespace Singularity.Expressions
{
    internal class ExpressionGenerator
    {
        public static readonly ParameterExpression ScopeParameter = Expression.Parameter(typeof(Scoped));
        internal static readonly MethodInfo CreateScopedExpressionMethod = typeof(ExpressionGenerator).GetRuntimeMethods().FirstOrDefault(x => x.Name == nameof(CreateScopedExpression));

        public Expression GenerateBaseExpression(Binding binding, InstanceFactory[] children, Scoped containerScope, SingularitySettings settings)
        {
            if (binding.Expression is AbstractBindingExpression)
            {
                return Expression.Constant(binding);
            }
            Expression expression = binding.Expression! is LambdaExpression lambdaExpression ? lambdaExpression.Body : binding.Expression;
            var parameterExpressionVisitor = new ParameterExpressionVisitor(children!);
            expression = parameterExpressionVisitor.Visit(expression);

            if (binding.NeedsDispose == DisposeBehavior.Always || settings.AutoDispose && binding.NeedsDispose != DisposeBehavior.Never && typeof(IDisposable).IsAssignableFrom(expression.Type))
            {
                MethodInfo method = Scoped.AddDisposableMethod.MakeGenericMethod(expression.Type);
                expression = Expression.Call(ScopeParameter, method, expression);
            }

            if (binding.Finalizer != null)
            {
                MethodInfo method = Scoped.AddFinalizerMethod.MakeGenericMethod(expression.Type);
                expression = Expression.Call(ScopeParameter, method, expression, Expression.Constant(binding));
            }

            return ApplyCaching(binding.Lifetime, containerScope, expression);
        }

        public Expression ApplyDecorators(Type dependencyType, Binding binding, InstanceFactory[] children, Expression[] decorators, Scoped containerScope)
        {
            Expression expression = binding.BaseExpression ?? throw new ArgumentNullException("binding.BaseExpression");
            if (decorators.Length > 0)
            {
                var body = new List<Expression>();
                ParameterExpression instanceParameter = Expression.Variable(dependencyType, $"{dependencyType} instance");
                body.Add(Expression.Assign(instanceParameter, Expression.Convert(expression, dependencyType)));

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
                expression = body.Count == 1 ? expression : Expression.Block(new[] { instanceParameter }, body);
                expression = ApplyCaching(binding.Lifetime, containerScope, expression);
            }

            return expression;
        }

        private static Expression ApplyCaching(Lifetime lifetime, Scoped containerScope, Expression expression)
        {
            switch (lifetime)
            {
                case Lifetime.Transient:
                    return expression;
                case Lifetime.PerContainer:
                    object singletonInstance = GetSingleton(containerScope, expression);
                    return Expression.Constant(singletonInstance, expression.Type);
                case Lifetime.PerScope:
                    MethodInfo method = CreateScopedExpressionMethod.MakeGenericMethod(expression.Type);
                    expression = (Expression)method.Invoke(null, new object[] { expression });
                    return expression;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static object GetSingleton(Scoped containerScope, Expression expression)
        {
            switch (expression)
            {
                case ConstantExpression constantExpression:
                    return constantExpression.Value;
                case NewExpression newExpression:
                    if (newExpression.Arguments.Count == 0)
                    {
                        return newExpression.Constructor.Invoke(null);
                    }
                    else
                    {
                        return ((Func<Scoped, object>)Expression.Lambda(expression, ScopeParameter).CompileFast())(containerScope);
                    }
                default:
                    return ((Func<Scoped, object>)Expression.Lambda(expression, ScopeParameter).CompileFast())(containerScope);
            }
        }

        public static Expression CreateScopedExpression<T>(Expression expression)
        {
            var factory = (Func<Scoped, T>)Expression.Lambda(expression, ScopeParameter).CompileFast();
            MethodInfo method = Scoped.GetOrAddScopedInstanceMethod.MakeGenericMethod(expression.Type);
            return Expression.Call(ScopeParameter, method, Expression.Constant(factory), Expression.Constant(expression.Type));
        }
    }
}
