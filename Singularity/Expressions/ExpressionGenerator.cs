using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.FastExpressionCompiler;
using Singularity.Graph;

namespace Singularity.Expressions
{
    internal class ExpressionGenerator
    {
        public static ParameterExpression ScopeParameter = Expression.Parameter(typeof(Scoped));
        internal static readonly MethodInfo CreateScopedExpressionMethod = typeof(ExpressionGenerator).GetRuntimeMethods().FirstOrDefault(x => x.Name == nameof(CreateScopedExpression));

        public Expression GenerateBaseExpression(ResolvedDependency dependency, InstanceFactory[] children, Scoped containerScope, SingularitySettings settings)
        {
            Expression expression = dependency.Binding.Expression! is LambdaExpression lambdaExpression ? lambdaExpression.Body : dependency.Binding.Expression;
            var parameterExpressionVisitor = new ParameterExpressionVisitor(children!);
            expression = parameterExpressionVisitor.Visit(expression);

            if (dependency.Binding.NeedsDispose == DisposeBehavior.Always || settings.AutoDispose && dependency.Binding.NeedsDispose != DisposeBehavior.Never && typeof(IDisposable).IsAssignableFrom(expression.Type))
            {
                MethodInfo method = Scoped.AddDisposableMethod.MakeGenericMethod(expression.Type);
                expression = Expression.Call(ScopeParameter, method, expression);
            }

            if (dependency.Binding.Finalizer != null)
            {
                MethodInfo method = Scoped.AddFinalizerMethod.MakeGenericMethod(expression.Type);
                expression = Expression.Call(ScopeParameter, method, expression, Expression.Constant(dependency.Binding));
            }

            return ApplyCaching(dependency.Binding.Lifetime, containerScope, expression);
        }

        public Expression ApplyDecorators(Type dependencyType, ResolvedDependency dependency, InstanceFactory[] children, ReadOnlyCollection<Expression> decorators, Scoped containerScope)
        {
            Expression expression = dependency.BaseExpression;
            if (decorators.Count > 0)
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
                expression = ApplyCaching(dependency.Binding.Lifetime, containerScope, expression);
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
                    object singletonInstance = ((Func<Scoped, object>)Expression.Lambda(expression, ScopeParameter).CompileFast())(containerScope);
                    return Expression.Constant(singletonInstance, expression.Type);
                case Lifetime.PerScope:
                    MethodInfo method = CreateScopedExpressionMethod.MakeGenericMethod(expression.Type);
                    expression = (Expression)method.Invoke(null, new object[] { expression });
                    return expression;
                default:
                    throw new ArgumentOutOfRangeException();
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
