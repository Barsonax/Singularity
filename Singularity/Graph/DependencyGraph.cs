using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Singularity.Bindings;
using Singularity.Collections;
using Singularity.Graph.Resolvers;

namespace Singularity.Graph
{
    internal sealed class DependencyGraph
    {
        private readonly IResolverPipeline _resolverPipeline;

        public DependencyGraph(ReadOnlyBindingConfig registrations, Scoped scope, SingularitySettings settings, DependencyGraph? parentDependencyGraph = null)
        {
            var dependencies = new Dictionary<Type, Dependency>(registrations.Registrations.Count);
            foreach (ReadonlyRegistration registration in registrations.Registrations)
            {
                var dependency = new Dependency(registration);
                foreach (Type registrationDependencyType in registration.DependencyTypes)
                {
                    dependencies.Add(registrationDependencyType, dependency);
                }
            }
            var resolvers = new IDependencyResolver[] { new EnumerableDependencyResolver(), new ExpressionDependencyResolver(), new LazyDependencyResolver(), new FactoryDependencyResolver(), new ConcreteDependencyResolver(), new OpenGenericResolver() };
            _resolverPipeline = new ResolverPipeline(dependencies, registrations.Decorators, resolvers, scope, settings, parentDependencyGraph?._resolverPipeline);
        }

        public Expression? GetResolvedExpression(Type type)
        {
            ResolvedDependency dependency = _resolverPipeline.GetDependency(type).Default;
            return _resolverPipeline.ResolveDependency(type, dependency).Expression;
        }

        public Func<Scoped, object>? GetResolvedFactory(Type type)
        {
            ResolvedDependency dependency = _resolverPipeline.GetDependency(type).Default;
            return _resolverPipeline.ResolveDependency(type, dependency).Factory;
        }
    }
}