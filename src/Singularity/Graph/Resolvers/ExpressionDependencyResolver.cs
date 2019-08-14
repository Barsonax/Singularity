using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Expressions;

namespace Singularity.Graph.Resolvers
{
    internal sealed class ExpressionDependencyResolver : IDependencyResolver
    {
        private static readonly MethodInfo GenericCreateLambdaMethod = typeof(ExpressionDependencyResolver).GetMethod(nameof(CreateLambda));

        public IEnumerable<ServiceBinding> Resolve(IResolverPipeline graph, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Expression<>) && type.GenericTypeArguments.Length == 1)
            {
                Type funcType = type.GenericTypeArguments[0];
                if (funcType.GetGenericTypeDefinition() == typeof(Func<>) && funcType.GenericTypeArguments.Length == 1)
                {
                    Type dependencyType = funcType.GenericTypeArguments[0];
                    MethodInfo method = GenericCreateLambdaMethod.MakeGenericMethod(dependencyType);
                    foreach (InstanceFactory instanceFactory in graph.ResolveAll(dependencyType))
                    {
                        var newBinding = new ServiceBinding(type, new BindingMetadata(), instanceFactory.Context.Expression);

                        var expression = (Expression)method.Invoke(null, new object[] { instanceFactory.Context });
                        var factory = new InstanceFactory(type, new ExpressionContext(expression), scoped => expression);
                        newBinding.Factories.Add(factory);
                        yield return newBinding;
                    }
                }
            }
        }

        public static LambdaExpression CreateLambda<T>(ReadOnlyExpressionContext context)
        {
            Expression expression = ExpressionCompiler.OptimizeExpression(context);
            return Expression.Lambda<Func<T>>(expression);
        }
    }
}