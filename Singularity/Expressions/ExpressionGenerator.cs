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
        public static ParameterExpression ScopeParameter = Expression.Parameter(typeof(Scoped));
        internal static readonly MethodInfo CreateScopedExpressionMethod = typeof(ExpressionGenerator).GetRuntimeMethods().FirstOrDefault(x => x.Name == nameof(CreateScopedExpression));

        public Expression GenerateDependencyExpression(ResolvedDependency dependency, Scoped containerScope, SingularitySettings settings)
        {
            Expression expression = dependency.Binding.Expression! is LambdaExpression lambdaExpression ? lambdaExpression.Body : dependency.Binding.Expression;
            var parameterExpressionVisitor = new ParameterExpressionVisitor(dependency.Children!);
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

            expression = ApplyDecorators(dependency, expression);

            return ApplyCaching(dependency, containerScope, expression);
        }

        private static Expression ApplyDecorators(ResolvedDependency dependency, Expression expression)
        {
            if (dependency.Registration.Decorators.Count > 0)
            {
                var body = new List<Expression>();
                ParameterExpression instanceParameter = Expression.Variable(dependency.Registration.DependencyType, $"{expression.Type} instance");
                body.Add(Expression.Assign(instanceParameter, Expression.Convert(expression, dependency.Registration.DependencyType)));

                if (dependency.Registration.Decorators.Count > 0)
                {
                    var decoratorExpressionVisitor = new DecoratorExpressionVisitor(dependency.Children!, instanceParameter.Type);
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
                expression = body.Count == 1 ? expression : Expression.Block(new[] {instanceParameter}, body);
            }

            return expression;
        }

        private static Expression ApplyCaching(ResolvedDependency dependency, Scoped containerScope, Expression expression)
        {
            switch (dependency.Binding.Lifetime)
            {
                case Lifetime.Transient:
                    return expression;
                case Lifetime.PerContainer:
                    object singletonInstance =
                        ((Func<Scoped, object>) Expression.Lambda(expression, ScopeParameter).CompileFast())(containerScope);
                    dependency.InstanceFactory = scope => singletonInstance;
                    return Expression.Constant(singletonInstance, dependency.Registration.DependencyType);
                case Lifetime.PerScope:
                    MethodInfo method = CreateScopedExpressionMethod.MakeGenericMethod(expression.Type);
                    expression = (Expression) method.Invoke(null, new object[] {expression});
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

    public readonly struct InstanceFactory
    {
        public Type Type { get; }
        public Func<Scoped, object> Factory { get; }

        public InstanceFactory(Type type, Func<Scoped, object> factory)
        {
            Type = type;
            Factory = factory;
        }
    }
}
