using Singularity.Collections;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity.Resolving.Generators
{
    /// <summary>
    /// Creates bindings for resolving the container or scope itself.
    /// </summary>
    public class ContainerServiceBindingGenerator : IGenericServiceGenerator
    {
        private static readonly MethodInfo _getContainer = typeof(ContainerServiceBindingGenerator).GetMethod(nameof(GetContainer), BindingFlags.NonPublic | BindingFlags.Static);

        private static Container GetContainer(Scoped scope)
        {
            return scope.Container;
        }

        public bool CanResolve(Type type)
        {
            return type == typeof(Container);
        }

        public IEnumerable<ServiceBinding> Wrap(IInstanceFactoryResolver resolver, Type targetType)
        {
            var expression = Expression.Call(_getContainer, Expression.Parameter(typeof(Scoped)));

            yield return new ServiceBinding(typeof(Container), BindingMetadata.GeneratedInstance, expression, typeof(Container), ConstructorResolvers.Default, Lifetimes.PerContainer, null, ServiceAutoDispose.Never);
        }
    }

    public class ScopedServiceGenerator : IGenericServiceGenerator
    {
        private static readonly MethodInfo _getScope = typeof(ScopedServiceGenerator).GetMethod(nameof(GetScope), BindingFlags.NonPublic | BindingFlags.Static);
        public bool CanResolve(Type type)
        {
            return type == typeof(Scoped) || type == typeof(IServiceProvider);
        }

        private static Scoped GetScope(Scoped scope)
        {
            return scope;
        }

        public IEnumerable<ServiceBinding> Wrap(IInstanceFactoryResolver resolver, Type targetType)
        {
            var expression = Expression.Call(_getScope, Expression.Parameter(typeof(Scoped)));
            yield return new ServiceBinding(new SinglyLinkedListNode<Type>(typeof(IServiceProvider)).Add(typeof(Scoped)), BindingMetadata.GeneratedInstance, expression, typeof(Scoped), ConstructorResolvers.Default, Lifetimes.PerContainer, null, ServiceAutoDispose.Never);
        }
    }
}
