using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Expressions;

namespace Singularity.Graph.Resolvers
{
    public class ContainerDependencyResolver : IDependencyResolver
    {
        private static readonly MethodInfo _getContainer = typeof(ContainerDependencyResolver).GetMethod(nameof(GetContainer), BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly MethodInfo _getScope = typeof(ContainerDependencyResolver).GetMethod(nameof(GetScope), BindingFlags.NonPublic | BindingFlags.Static);

        private readonly List<IMatch> Exclusions = new List<IMatch>();

        public IEnumerable<ServiceBinding> Resolve(IResolverPipeline graph, Type type)
        {
            foreach (IMatch resolverExclusion in Exclusions)
            {
                if (resolverExclusion.Match(type)) yield break;
            }
            if (type == typeof(Container))
            {
                Expression expression = Expression.Call(null, _getContainer, ExpressionGenerator.ScopeParameter);
                yield return new ServiceBinding(type, BindingMetadata.GeneratedInstance, expression, graph.Settings.ConstructorResolver, Lifetimes.PerContainer, null, ServiceAutoDispose.Never);
            }

            if (type == typeof(Scoped) || type == typeof(IServiceProvider))
            {
                Expression expression = Expression.Call(null, _getScope, ExpressionGenerator.ScopeParameter);
                yield return new ServiceBinding(type, BindingMetadata.GeneratedInstance, expression, graph.Settings.ConstructorResolver, Lifetimes.PerContainer, null, ServiceAutoDispose.Never);
            }
        }

        public void Exclude(string pattern)
        {
            Exclusions.Add(new PatternMatch(pattern));
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
