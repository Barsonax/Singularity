using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Bindings;
using Singularity.Expressions;
using Singularity.Resolvers;

namespace Singularity.Generators
{
    /// <summary>
    /// Creates bindings so that the binding can be resolved as a <see cref="Lazy{T}"/>
    /// </summary>
    public sealed class LazyServiceBindingGenerator : IServiceBindingGenerator
    {
        /// <inheritdoc />
        public IEnumerable<ServiceBinding> TryGenerate(IContainerContext context, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Lazy<>))
            {
                Type funcType = typeof(Func<>).MakeGenericType(type.GenericTypeArguments[0]);
                Type lazyType = typeof(Lazy<>).MakeGenericType(type.GenericTypeArguments[0]);

                ConstructorInfo constructor = lazyType.GetConstructor(new[] { funcType });

                foreach (InstanceFactory factory in context.TryResolveAll(funcType))
                {
                    var expressionContext = (ExpressionContext)factory.Context;
                    expressionContext.Expression = Expression.New(constructor, factory.Context.Expression);
                    var newBinding = new ServiceBinding(type, BindingMetadata.GeneratedInstance, expressionContext.Expression, type, ConstructorResolvers.Default, Lifetimes.Transient);
                    newBinding.Factories.Add(new InstanceFactory(type, expressionContext));
                    yield return newBinding;
                }
            }
        }
    }
}
