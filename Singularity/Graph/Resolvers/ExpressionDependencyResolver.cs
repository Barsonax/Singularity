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

        public IEnumerable<Dependency>? Resolve(IResolverPipeline graph, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Expression<>) && type.GenericTypeArguments.Length == 1)
            {
                var funcType = type.GenericTypeArguments[0];
                if (funcType.GetGenericTypeDefinition() == typeof(Func<>) && funcType.GenericTypeArguments.Length == 1)
                {
                    Type dependencyType = funcType.GenericTypeArguments[0];
                    Dependency dependency = graph.GetDependency(dependencyType);

                    var expressions = new List<Expression>();
                    foreach (ResolvedDependency resolvedDependency in dependency.ResolvedDependencies.Array)
                    {
                        graph.ResolveDependency(resolvedDependency);
                        expressions.Add(resolvedDependency.Expression);
                    }

                    var expressionDependency = new Dependency(type, expressions, CreationMode.Transient);
                    var method = GenericCreateLambdaMethod.MakeGenericMethod(dependencyType);
                    for (int i = 0; i < expressionDependency.ResolvedDependencies.Array.Length; i++)
                    {
                        Expression expression = (Expression)method.Invoke(null, new object[] { expressions[i] });
                        expressionDependency.ResolvedDependencies.Array[i].Expression = expression;
                        expressionDependency.ResolvedDependencies.Array[i].InstanceFactory = scoped => expression;
                    }
                    return new[] { expressionDependency };
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