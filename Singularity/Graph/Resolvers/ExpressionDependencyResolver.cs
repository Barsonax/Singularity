using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity.Graph.Resolvers
{
    internal class ExpressionDependencyResolver : IDependencyResolver
    {
        private static readonly MethodInfo GenericCreateLambdaMethod;
        static ExpressionDependencyResolver()
        {
            GenericCreateLambdaMethod = typeof(ExpressionDependencyResolver).GetMethod(nameof(CreateLambda));
        }

        public Dependency? Resolve(IResolverPipeline graph, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Expression<>) && type.GenericTypeArguments.Length == 1)
            {
                Type funcType = type.GenericTypeArguments[0];
                if (funcType.GetGenericTypeDefinition() == typeof(Func<>) && funcType.GenericTypeArguments.Length == 1)
                {
                    Type dependencyType = funcType.GenericTypeArguments[0];
                    Dependency dependency = graph.GetDependency(dependencyType);

                    var expressions = new List<Expression>();
                    foreach (ResolvedDependency resolvedDependency in dependency.ResolvedDependencies.Array)
                    {
                        expressions.Add(graph.ResolveDependency(dependencyType, resolvedDependency).Expression);
                    }

                    var expressionDependency = new Dependency(new[] { type }, expressions, Lifetime.Transient);
                    MethodInfo method = GenericCreateLambdaMethod.MakeGenericMethod(dependencyType);
                    for (var i = 0; i < expressionDependency.ResolvedDependencies.Array.Length; i++)
                    {
                        var expression = (Expression)method.Invoke(null, new object[] { expressions[i] });
                        var factory = new InstanceFactory(type, expression, scoped => expression);
                        expressionDependency.ResolvedDependencies.Array[i].Factories.Add(factory);
                    }
                    return expressionDependency;
                }
            }
            return null;
        }

        public static LambdaExpression CreateLambda<T>(Expression expression)
        {
            return Expression.Lambda<Func<T>>(expression);
        }
    }
}