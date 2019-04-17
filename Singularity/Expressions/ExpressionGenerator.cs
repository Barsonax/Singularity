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
            var parameterExpressionVisitor = new ParameterExpressionVisitor(dependency.Children);
            expression = parameterExpressionVisitor.Visit(expression);

            if (dependency.Binding.NeedsDispose == Dispose.Always || settings.AutoDispose && dependency.Binding.NeedsDispose != Dispose.Never && typeof(IDisposable).IsAssignableFrom(expression.Type))
            {
                MethodInfo method = Scoped.AddDisposableMethod.MakeGenericMethod(expression.Type);
                expression = Expression.Call(ScopeParameter, method, expression);
            }

            if (dependency.Binding.Finalizer != null)
            {
                MethodInfo method = Scoped.AddFinalizerMethod.MakeGenericMethod(expression.Type);
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
                    //var scopedFactory = (Func<Scoped, object>)Expression.Lambda(expression, ScopeParameter).CompileFast();
                    //var factory = new InstanceFactory(dependency.Registration.DependencyType, scopedFactory);
                    MethodInfo method = CreateScopedExpressionMethod.MakeGenericMethod(expression.Type);
                    expression = (Expression)method.Invoke(null, new object[] { dependency, expression });
                    //expression = Expression.Call(ScopeParameter, method, Expression.Lambda(expression, ScopeParameter), Expression.Constant(dependency.Registration.DependencyType));
                    return expression;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Expression CreateScopedExpression<T>(ResolvedDependency dependency, Expression expression)
        {
            var factory = (Func<Scoped, T>)Expression.Lambda(expression, ScopeParameter).CompileFast();
            MethodInfo method = Scoped.GetOrAddScopedInstanceMethod.MakeGenericMethod(dependency.Registration.DependencyType);
            return Expression.Call(ScopeParameter, method, Expression.Constant(factory), Expression.Constant(dependency.Registration.DependencyType));
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
