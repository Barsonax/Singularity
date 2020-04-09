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

        public IEnumerable<ServiceBinding> Wrap<TTarget>(IInstanceFactoryResolver resolver)
        {
            var innerType = typeof(TTarget).GetGenericArguments()[0];

            foreach (var item in resolver.FindOrGenerateApplicableBindings(innerType))
            {                
                var factory = resolver.TryResolveDependency(innerType, item);
                if (factory != null)
                {
                    var expression = Expression.Lambda<TTarget>(factory.Context.Expression);
                    yield return new ServiceBinding(typeof(TTarget), BindingMetadata.GeneratedInstance, expression, typeof(TTarget), ConstructorResolvers.Default, Lifetimes.PerContainer);
                }
            }
        }
    }
}
