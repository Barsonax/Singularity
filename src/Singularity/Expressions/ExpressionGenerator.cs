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
        internal static readonly MethodInfo CreateScopedExpressionMethod = typeof(ExpressionGenerator).GetRuntimeMethods().FirstOrDefault(x => x.Name == nameof(CreateScopedExpression));

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

            if (serviceBinding.NeedsDispose == DisposeBehavior.Always || settings.AutoDispose && serviceBinding.NeedsDispose != DisposeBehavior.Never && typeof(IDisposable).IsAssignableFrom(context.Expression.Type))
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

            ApplyCaching(serviceBinding.Lifetime, containerScope, context);
            return context;
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
                ApplyCaching(serviceBinding.Lifetime, containerScope, context);
            }

            return context;
        }

        private static void ApplyCaching(Lifetime lifetime, Scoped containerScope, ExpressionContext context)
        {
            switch (lifetime)
            {
                case Lifetime.Transient:
                    break;
                case Lifetime.PerContainer:
                    object singletonInstance = GetSingleton(containerScope, context.Expression);
                    context.Expression = Expression.Constant(singletonInstance, context.Expression.Type);
                    context.ScopedExpressions.Clear();
                    break;
                case Lifetime.PerScope:
                    MethodInfo method = CreateScopedExpressionMethod.MakeGenericMethod(context.Expression.Type);
                    context.Expression = (Expression)method.Invoke(null, new object[] { context.Expression });
                    context.ScopedExpressions.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
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
            var factory = Expression.Lambda(expression, ScopeParameter).CompileFast<Func<Scoped, T>>();
            MethodInfo method = Scoped.GetOrAddScopedInstanceMethod.MakeGenericMethod(expression.Type);
            return Expression.Call(ScopeParameter, method, Expression.Constant(factory), Expression.Constant(expression.Type));
        }
    }
}
