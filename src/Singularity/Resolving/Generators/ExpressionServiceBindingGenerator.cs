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

        public IEnumerable<ServiceBinding> Wrap<TTarget>(IInstanceFactoryResolver resolver)
        {
            var funcType =  typeof(TTarget).GetGenericArguments()[0];
            var funcReturnType = funcType.GetGenericArguments()[0];

            foreach (var item in resolver.FindOrGenerateApplicableBindings(funcReturnType))
            {
                var factory = resolver.TryResolveDependency(funcReturnType, item);
                if(factory != null)
                {
                    var expression = Expression.Constant(Expression.Lambda(funcType, factory.Context.Expression));
                    yield return new ServiceBinding(typeof(TTarget), BindingMetadata.GeneratedInstance, expression, typeof(TTarget), ConstructorResolvers.Default, Lifetimes.PerContainer);
                }
            }
        }
    }
}