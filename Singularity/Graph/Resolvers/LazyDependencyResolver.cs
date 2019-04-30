using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity.Graph.Resolvers
{
    internal sealed class LazyDependencyResolver : IDependencyResolver
    {
        public IEnumerable<Binding> Resolve(IResolverPipeline graph, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Lazy<>))
            {
                Type funcType = typeof(Func<>).MakeGenericType(type.GenericTypeArguments[0]);
                Type lazyType = typeof(Lazy<>).MakeGenericType(type.GenericTypeArguments[0]);
                Registration factoryDependency = graph.GetDependency(funcType);

                ConstructorInfo constructor = lazyType.GetConstructor(new[] { funcType });

                foreach (Binding binding in factoryDependency.Bindings)
                {
                    NewExpression baseExpression = Expression.New(constructor, graph.ResolveDependency(lazyType, binding).Expression);
                    var newBinding = new Binding(new BindingMetadata(type), baseExpression);
                    newBinding.Factories.Add(new InstanceFactory(type, baseExpression));
                    yield return newBinding;
                }
            }
        }
    }
}
