using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Expressions;

namespace Singularity.Graph.Resolvers
{
    /// <summary>
    /// Creates bindings so that the expression itself of a binding can be resolved
    /// </summary>
    public sealed class ExpressionServiceBindingGenerator : IServiceBindingGenerator
    {
        private static readonly MethodInfo GenericCreateLambdaMethod = typeof(ExpressionServiceBindingGenerator).GetMethod(nameof(CreateLambda));

        /// <inheritdoc />
        public IEnumerable<ServiceBinding> Resolve(IResolverPipeline graph, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Expression<>) && type.GenericTypeArguments.Length == 1)
            {
                Type funcType = type.GenericTypeArguments[0];
                if (funcType.GetGenericTypeDefinition() == typeof(Func<>) && funcType.GenericTypeArguments.Length == 1)
                {
                    Type dependencyType = funcType.GenericTypeArguments[0];
                    MethodInfo method = GenericCreateLambdaMethod.MakeGenericMethod(dependencyType);
                    foreach (InstanceFactory instanceFactory in graph.TryResolveAll(dependencyType))
                    {
                        var newBinding = new ServiceBinding(type, BindingMetadata.GeneratedInstance, instanceFactory.Context.Expression, instanceFactory.Context.Expression.Type, graph.Settings.ConstructorResolver);

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