using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Singularity.Exceptions;
using Singularity.Expressions;

namespace Singularity.Graph.Resolvers
{
    internal class ResolverPipeline : IResolverPipeline
    {
        private static readonly ReadOnlyCollection<Expression> EmptyDecorators = new ReadOnlyCollection<Expression>(new Expression[0]);
        IReadOnlyDictionary<Type, Dependency> IResolverPipeline.Dependencies => Dependencies;
        private Dictionary<Type, Dependency> Dependencies { get; }
        private ReadOnlyDictionary<Type, ReadOnlyCollection<Expression>> Decorators { get; }
        public object SyncRoot { get; }
        private readonly IDependencyResolver[] _resolvers;
        private readonly IResolverPipeline? _parentPipeline;
        private readonly Scoped _containerScope;
        private readonly ExpressionGenerator _expressionGenerator = new ExpressionGenerator();
        private readonly SingularitySettings _settings;

        public ResolverPipeline(Dictionary<Type, Dependency> dependencies, ReadOnlyDictionary<Type, ReadOnlyCollection<Expression>> decorators, IDependencyResolver[] resolvers, Scoped containerScope, SingularitySettings settings, IResolverPipeline? parentPipeline)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _resolvers = resolvers ?? throw new ArgumentNullException(nameof(resolvers));
            _parentPipeline = parentPipeline;
            SyncRoot = parentPipeline?.SyncRoot ?? new object();
            _containerScope = containerScope ?? throw new ArgumentNullException(nameof(containerScope));
            Dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
            Decorators = decorators ?? throw new ArgumentNullException(nameof(decorators));

            if (parentPipeline != null)
            {
                CheckChildBindings(parentPipeline, dependencies, SyncRoot);
            }
        }

        public Dependency? TryGetDependency(Type type)
        {
            lock (SyncRoot)
            {
                if (Dependencies.TryGetValue(type, out Dependency parent)) return parent;

                foreach (IDependencyResolver dependencyResolver in _resolvers)
                {
                    Dependency? resolvedDependency = dependencyResolver.Resolve(this, type);
                    if (resolvedDependency != null)
                    {
                        foreach (Type registrationDependencyType in resolvedDependency.Registration.DependencyTypes)
                        {
                            Dependencies.Add(registrationDependencyType, resolvedDependency);
                        }
                        return TryGetDependency(type);
                    }
                }

                return _parentPipeline?.TryGetDependency(type);
            }
        }

        public Dependency GetDependency(Type type)
        {
            Dependency? dependency = TryGetDependency(type);
            if (dependency == null) throw new DependencyNotFoundException(type);
            return dependency;
        }

        public InstanceFactory ResolveDependency(Type type, ResolvedDependency dependency) => ResolveDependency(type, dependency, new CircularDependencyDetector());

        private InstanceFactory ResolveDependency(Type type, ResolvedDependency dependency, CircularDependencyDetector circularDependencyDetector)
        {
            circularDependencyDetector.Enter(type);
            GenerateBaseExpression(dependency, circularDependencyDetector);
            InstanceFactory factory = GenerateInstanceFactory(type, dependency, circularDependencyDetector);
            circularDependencyDetector.Leave(type);
            return factory;
        }

        private void GenerateBaseExpression(ResolvedDependency dependency, CircularDependencyDetector circularDependencyDetector)
        {
            if (dependency.BaseExpression == null)
            {
                lock (dependency)
                {
                    if (dependency.BaseExpression == null)
                    {
                        ParameterExpression[] parameters = dependency.Binding.Expression.GetParameterExpressions().Where(x => x.Type != ExpressionGenerator.ScopeParameter.Type).ToArray();
                        var factories = new InstanceFactory[parameters.Length];
                        for (var i = 0; i < parameters.Length; i++)
                        {
                            ParameterExpression parameter = parameters[i];
                            ResolvedDependency child = GetDependency(parameter.Type).Default;
                            factories[i] = ResolveDependency(parameter.Type, child, circularDependencyDetector);
                        }

                        if (dependency.ResolveError != null) throw dependency.ResolveError;
                        dependency.BaseExpression = _expressionGenerator.GenerateBaseExpression(dependency, factories, _containerScope, _settings);
                    }
                }
            }
        }

        private InstanceFactory GenerateInstanceFactory(Type type, ResolvedDependency dependency, CircularDependencyDetector circularDependencyDetector)
        {
            lock (dependency)
            {
                if (!dependency.TryGetInstanceFactory(type, out InstanceFactory factory))
                {
                    ReadOnlyCollection<Expression> decorators = FindDecorators(type);
                    ParameterExpression[] parameters = decorators.GetParameterExpressions().Where(x => x.Type != ExpressionGenerator.ScopeParameter.Type && x.Type != type).ToArray();
                    var childFactories = new InstanceFactory[parameters.Length];
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        ParameterExpression parameter = parameters[i];
                        ResolvedDependency child = GetDependency(parameter.Type).Default;
                        childFactories[i] = ResolveDependency(parameter.Type, child, circularDependencyDetector);
                    }

                    Expression expression = _expressionGenerator.ApplyDecorators(type, dependency, childFactories, decorators, _containerScope);
                    factory = new InstanceFactory(type, expression);
                    dependency.Factories.Add(factory);
                }
                return factory;
            }
        }

        private ReadOnlyCollection<Expression> FindDecorators(Type type)
        {
            return Decorators.TryGetValue(type, out ReadOnlyCollection<Expression> decorators) ? decorators : EmptyDecorators;
        }

        private static void CheckChildBindings(IResolverPipeline parentDependencyGraph, Dictionary<Type, Dependency> bindings, object syncRoot)
        {
            lock (syncRoot)
            {
                foreach (Dependency childBinding in bindings.Values)
                {
                    foreach (Type type in childBinding.Registration.DependencyTypes)
                    {
                        if (parentDependencyGraph.Dependencies.TryGetValue(type, out Dependency _))
                        {
                            throw new RegistrationAlreadyExistsException($"Dependency {type} was already registered in the parent graph!");
                        }
                        else if (type.IsGenericType)
                        {
                            if (parentDependencyGraph.Dependencies.TryGetValue(type.GetGenericTypeDefinition(), out Dependency _))
                            {
                                throw new RegistrationAlreadyExistsException($"Dependency {type} was already registered as a open generic in the parent graph!");
                            }
                        }
                    }

                }
            }
        }
    }
}
