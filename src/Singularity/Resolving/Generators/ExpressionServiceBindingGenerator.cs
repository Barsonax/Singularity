using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Singularity.Resolving.Generators
{
    /// <summary>
    /// Creates bindings so that the expression itself of a binding can be resolved
    /// </summary>
    public sealed class ExpressionServiceBindingGenerator : IGenericServiceGenerator
    {
        public bool CanResolve(Type type)
        {
            if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Expression<>) && type.GenericTypeArguments.Length == 1)
            {
                Type funcType = type.GenericTypeArguments[0];
                return funcType.GetGenericTypeDefinition() == typeof(Func<>) && funcType.GenericTypeArguments.Length == 1;
            }
            return false;
        }

        public IEnumerable<ServiceBinding> Wrap(IInstanceFactoryResolver resolver, Type targetType)
        {
            var innerType = targetType.GetGenericArguments()[0].GetGenericArguments()[0];

            foreach (var item in resolver.FindOrGenerateApplicableBindings(innerType))
            {
                var factory = resolver.TryResolveDependency(innerType, item);
                if(factory != null)
                {
                    yield return new ServiceBinding(targetType, BindingMetadata.GeneratedInstance, Expression.Constant(factory.Context.Expression), targetType, ConstructorResolvers.Default, Lifetimes.PerContainer);
                }
            }
        }
    }
}