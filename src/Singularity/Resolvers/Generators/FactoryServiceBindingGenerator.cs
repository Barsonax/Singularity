using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Singularity.Expressions;

namespace Singularity.Resolvers.Generators
{
    /// <summary>
    /// Creates bindings so that the factory itself of a binding can be resolved
    /// </summary>
    public sealed class FactoryServiceBindingGenerator : IServiceBindingGenerator
    {
        /// <inheritdoc />
        public IEnumerable<ServiceBinding> Resolve(IInstanceFactoryResolver resolver, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Func<>))
            {
                Type dependencyType = type.GenericTypeArguments[0];

                foreach (InstanceFactory factory in resolver.TryResolveAll(dependencyType))
                {
                    LambdaExpression baseExpression = Expression.Lambda(factory.Context.Expression);

                    yield return new ServiceBinding(type, BindingMetadata.GeneratedInstance, baseExpression, type, ConstructorResolvers.Default, Lifetimes.Transient)
                    {
                        BaseExpression = new ExpressionContext(baseExpression)
                    };
                }
            }
        }
    }
}
