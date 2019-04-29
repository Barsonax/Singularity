using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Singularity.Graph.Resolvers
{
    internal class FactoryDependencyResolver : IDependencyResolver
    {
        public IEnumerable<Binding> Resolve(IResolverPipeline graph, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Func<>))
            {
                Type dependencyType = type.GenericTypeArguments[0];
                Registration dependency = graph.GetDependency(dependencyType);

                foreach (Binding binding in dependency.Bindings)
                {
                    InstanceFactory factory = graph.ResolveDependency(dependencyType, binding);
                    LambdaExpression baseExpression = Expression.Lambda(factory.Expression);

                    yield return new Binding(new BindingMetadata(type), baseExpression)
                    {
                        BaseExpression = baseExpression
                    };
                }
            }
        }
    }
}
