using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Expressions;

namespace Singularity.Graph.Resolvers
{
    internal sealed class LazyDependencyResolver : IDependencyResolver
    {
        public IEnumerable<ServiceBinding> Resolve(IResolverPipeline graph, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Lazy<>))
            {
                Type funcType = typeof(Func<>).MakeGenericType(type.GenericTypeArguments[0]);
                Type lazyType = typeof(Lazy<>).MakeGenericType(type.GenericTypeArguments[0]);

                ConstructorInfo constructor = lazyType.GetConstructor(new[] { funcType });

                foreach (InstanceFactory factory in graph.ResolveAll(funcType))
                {
                    var context = (ExpressionContext)factory.Context;
                    context.Expression = Expression.New(constructor, factory.Context.Expression);
                    var newBinding = new ServiceBinding(type, BindingMetadata.GeneratedInstance, context.Expression);
                    newBinding.Factories.Add(new InstanceFactory(type, context));
                    yield return newBinding;
                }
            }
        }
    }
}
