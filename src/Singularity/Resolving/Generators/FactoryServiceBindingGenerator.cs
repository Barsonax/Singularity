using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Singularity.Resolving.Generators
{
    /// <summary>
    /// Creates bindings so that the factory itself of a binding can be resolved
    /// </summary>
    public sealed class FactoryServiceBindingGenerator : IGenericServiceGenerator
    {
        public bool CanResolve(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Func<>);
        }

        public IEnumerable<ServiceBinding> Wrap(IInstanceFactoryResolver resolver, Type targetType)
        {
            var innerType = targetType.GetGenericArguments()[0];

            foreach (var item in resolver.FindOrGenerateApplicableBindings(innerType))
            {                
                var factory = resolver.TryResolveDependency(innerType, item);
                if (factory != null)
                {
                    LambdaExpression baseExpression = Expression.Lambda(factory.Context.Expression);
                    yield return new ServiceBinding(targetType, BindingMetadata.GeneratedInstance, baseExpression, targetType, ConstructorResolvers.Default, Lifetimes.PerContainer);
                }
            }
        }
    }
}
