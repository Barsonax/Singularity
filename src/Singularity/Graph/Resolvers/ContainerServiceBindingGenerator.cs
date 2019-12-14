using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Expressions;

namespace Singularity.Graph.Resolvers
{
    /// <summary>
    /// Creates bindings for resolving the container or scope itself.
    /// </summary>
    public class ContainerServiceBindingGenerator : IServiceBindingGenerator
    {
        private static readonly MethodInfo _getContainer = typeof(ContainerServiceBindingGenerator).GetMethod(nameof(GetContainer), BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly MethodInfo _getScope = typeof(ContainerServiceBindingGenerator).GetMethod(nameof(GetScope), BindingFlags.NonPublic | BindingFlags.Static);

        /// <inheritdoc />
        public IEnumerable<ServiceBinding> Resolve(IResolverPipeline graph, Type type)
        {
            if (type == typeof(Container))
            {
                Expression expression = Expression.Call(null, _getContainer, ExpressionGenerator.ScopeParameter);
                yield return new ServiceBinding(type, BindingMetadata.GeneratedInstance, expression, type, ConstructorResolvers.Default, Lifetimes.PerContainer, null, ServiceAutoDispose.Never);
            }

            if (type == typeof(Scoped) || type == typeof(IServiceProvider))
            {
                Expression expression = Expression.Call(null, _getScope, ExpressionGenerator.ScopeParameter);
                yield return new ServiceBinding(type, BindingMetadata.GeneratedInstance, expression, type, ConstructorResolvers.Default, Lifetimes.PerContainer, null, ServiceAutoDispose.Never);
            }
        }

        private static Container GetContainer(Scoped scope)
        {
            return scope.Container;
        }

        private static Scoped GetScope(Scoped scope)
        {
            return scope;
        }
    }
}
