using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

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
                    NewExpression baseExpression = Expression.New(constructor, factory.Expression);
                    var newBinding = new ServiceBinding(type, new BindingMetadata(), baseExpression);
                    newBinding.Factories.Add(new InstanceFactory(type, baseExpression));
                    yield return newBinding;
                }
            }
        }
    }
}
