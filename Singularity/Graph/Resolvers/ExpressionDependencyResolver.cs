using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Bindings;

namespace Singularity.Graph.Resolvers
{
    internal class ExpressionDependencyResolver : IDependencyResolver
    {
        private static readonly MethodInfo GenericCreateLambdaMethod;
        static ExpressionDependencyResolver()
        {
            GenericCreateLambdaMethod = typeof(ExpressionDependencyResolver).GetMethod(nameof(CreateLambda));
        }

        public IEnumerable<Binding> Resolve(IResolverPipeline graph, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Expression<>) && type.GenericTypeArguments.Length == 1)
            {
                Type funcType = type.GenericTypeArguments[0];
                if (funcType.GetGenericTypeDefinition() == typeof(Func<>) && funcType.GenericTypeArguments.Length == 1)
                {
                    Type dependencyType = funcType.GenericTypeArguments[0];
                    Registration registration = graph.GetDependency(dependencyType);

                    MethodInfo method = GenericCreateLambdaMethod.MakeGenericMethod(dependencyType);

                    foreach (Binding binding in registration.Bindings)
                    {
                        Expression baseExpression = graph.ResolveDependency(dependencyType, binding).Expression;
                        var newBinding = new Binding(new BindingMetadata(type), baseExpression);

                        var expression = (Expression)method.Invoke(null, new object[] { baseExpression });
                        var factory = new InstanceFactory(type, expression, scoped => expression);
                        newBinding.Factories.Add(factory);
                        yield return newBinding;
                    }
                }
            }
        }

        public static LambdaExpression CreateLambda<T>(Expression expression)
        {
            return Expression.Lambda<Func<T>>(expression);
        }
    }
}