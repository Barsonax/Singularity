using System;
using System.Linq.Expressions;
using Singularity.Collections;
using Singularity.Graph.Resolvers;

namespace Singularity.Graph
{
    internal sealed class DependencyGraph
    {
        private readonly IResolverPipeline _resolverPipeline;

        public DependencyGraph(RegistrationStore registrations, Scoped scope, SingularitySettings settings, DependencyGraph? parentDependencyGraph = null)
        {
            var resolvers = new IDependencyResolver[] { new EnumerableDependencyResolver(), new ExpressionDependencyResolver(), new LazyDependencyResolver(), new FactoryDependencyResolver(), new ConcreteDependencyResolver(), new OpenGenericResolver() };
            _resolverPipeline = new ResolverPipeline(registrations, resolvers, scope, settings, parentDependencyGraph?._resolverPipeline);
        }

        public Expression? GetResolvedExpression(Type type)
        {
            Binding dependency = _resolverPipeline.GetDependency(type).Default;
            return _resolverPipeline.ResolveDependency(type, dependency).Expression;
        }

        public Func<Scoped, object>? GetResolvedFactory(Type type)
        {
            Binding dependency = _resolverPipeline.GetDependency(type).Default;
            return _resolverPipeline.ResolveDependency(type, dependency).Factory;
        }
    }
}