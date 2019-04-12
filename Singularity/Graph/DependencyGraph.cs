using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Singularity.Bindings;
using Singularity.Graph.Resolvers;

namespace Singularity.Graph
{
    internal sealed class DependencyGraph
    {
        private readonly IResolverPipeline _resolverPipeline;

        public DependencyGraph(ReadOnlyCollection<ReadonlyRegistration> registrations, Scoped scope, DependencyGraph? parentDependencyGraph = null)
        {
            var dependencies = new Dictionary<Type, Dependency>(registrations.Count);
            foreach (ReadonlyRegistration registration in registrations)
            {
                dependencies.Add(registration.DependencyType, new Dependency(registration));
            }
            var resolvers = new IDependencyResolver[] { new EnumerableDependencyResolver(), new LazyDependencyResolver(), new FactoryDependencyResolver(), new ConcreteDependencyResolver(), new OpenGenericResolver() };
            _resolverPipeline = new ResolverPipeline(dependencies, resolvers, scope, parentDependencyGraph?._resolverPipeline);
        }

        public Expression? GetResolvedExpression(Type type)
        {
            ResolvedDependency dependency = _resolverPipeline.GetDependency(type).Default;
            _resolverPipeline.ResolveDependency(dependency);
            return dependency.Expression;
        }

        public Func<Scoped, object>? GetResolvedFactory(Type type)
        {
            ResolvedDependency dependency = _resolverPipeline.GetDependency(type).Default;
            _resolverPipeline.ResolveDependency(dependency);
            return dependency.InstanceFactory;
        }
    }
}