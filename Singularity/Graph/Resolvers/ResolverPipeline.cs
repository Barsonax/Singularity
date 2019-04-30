using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Singularity.Collections;
using Singularity.Exceptions;
using Singularity.Expressions;

namespace Singularity.Graph.Resolvers
{
    internal sealed class ResolverPipeline : IResolverPipeline
    {
        IReadOnlyDictionary<Type, Registration> IResolverPipeline.Dependencies => RegistrationStore.Registrations;
        private RegistrationStore RegistrationStore { get; }
        public object SyncRoot { get; }
        private readonly IDependencyResolver[] _resolvers;
        private readonly IResolverPipeline? _parentPipeline;
        private readonly Scoped _containerScope;
        private readonly ExpressionGenerator _expressionGenerator = new ExpressionGenerator();
        private readonly SingularitySettings _settings;

        public ResolverPipeline(RegistrationStore registrationStore, IDependencyResolver[] resolvers, Scoped containerScope, SingularitySettings settings, IResolverPipeline? parentPipeline)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _resolvers = resolvers ?? throw new ArgumentNullException(nameof(resolvers));
            _parentPipeline = parentPipeline;
            SyncRoot = parentPipeline?.SyncRoot ?? new object();
            _containerScope = containerScope ?? throw new ArgumentNullException(nameof(containerScope));
            RegistrationStore = registrationStore ?? throw new ArgumentNullException(nameof(registrationStore));

            if (parentPipeline != null)
            {
                CheckChildRegistrations(parentPipeline, registrationStore.Registrations, SyncRoot);
            }
        }

        public Registration? TryGetDependency(Type type)
        {
            lock (SyncRoot)
            {
                if (RegistrationStore.Registrations.TryGetValue(type, out Registration parent)) return parent;

                foreach (IDependencyResolver dependencyResolver in _resolvers)
                {
                    Binding[] bindings = dependencyResolver.Resolve(this, type).ToArray();
                    if (bindings.Length > 0)
                    {
                        foreach (Binding binding in bindings)
                        {
                            RegistrationStore.AddBinding(binding);
                        }
                        return TryGetDependency(type);
                    }
                }

                return _parentPipeline?.TryGetDependency(type);
            }
        }

        public Registration GetDependency(Type type)
        {
            Registration? dependency = TryGetDependency(type);
            if (dependency == null) throw new DependencyNotFoundException(type);
            return dependency;
        }

        public InstanceFactory ResolveDependency(Type type, Binding dependency) => ResolveDependency(type, dependency, new CircularDependencyDetector());

        private InstanceFactory ResolveDependency(Type type, Binding dependency, CircularDependencyDetector circularDependencyDetector)
        {
            circularDependencyDetector.Enter(type);
            GenerateBaseExpression(dependency, circularDependencyDetector);
            InstanceFactory factory = GenerateInstanceFactory(type, dependency, circularDependencyDetector);
            circularDependencyDetector.Leave(type);
            return factory;
        }

        private void GenerateBaseExpression(Binding binding, CircularDependencyDetector circularDependencyDetector)
        {
            if (binding.BaseExpression == null)
            {
                lock (binding)
                {
                    if (binding.BaseExpression == null)
                    {
                        ParameterExpression[] parameters = binding.Expression.GetParameterExpressions().Where(x => x.Type != ExpressionGenerator.ScopeParameter.Type).ToArray();
                        var factories = new InstanceFactory[parameters.Length];
                        for (var i = 0; i < parameters.Length; i++)
                        {
                            ParameterExpression parameter = parameters[i];
                            Binding child = GetDependency(parameter.Type).Default;
                            factories[i] = ResolveDependency(parameter.Type, child, circularDependencyDetector);
                        }

                        if (binding.ResolveError != null) throw binding.ResolveError;
                        binding.BaseExpression = _expressionGenerator.GenerateBaseExpression(binding, factories, _containerScope, _settings);
                    }
                }
            }
        }


        private InstanceFactory GenerateInstanceFactory(Type type, Binding dependency, CircularDependencyDetector circularDependencyDetector)
        {
            lock (dependency)
            {
                if (!dependency.TryGetInstanceFactory(type, out InstanceFactory factory))
                {
                    Expression[] decorators = FindDecorators(type);
                    ParameterExpression[] parameters = decorators.GetParameterExpressions().Where(x => x.Type != ExpressionGenerator.ScopeParameter.Type && x.Type != type).ToArray();
                    var childFactories = new InstanceFactory[parameters.Length];
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        ParameterExpression parameter = parameters[i];
                        Binding child = GetDependency(parameter.Type).Default;
                        childFactories[i] = ResolveDependency(parameter.Type, child, circularDependencyDetector);
                    }

                    Expression expression = _expressionGenerator.ApplyDecorators(type, dependency, childFactories, decorators, _containerScope);
                    factory = new InstanceFactory(type, expression);
                    dependency.Factories.Add(factory);
                }
                return factory;
            }
        }

        private Expression[] FindDecorators(Type type)
        {
            return RegistrationStore.Decorators.TryGetValue(type, out ArrayList<Expression> decorators) ? decorators.Array : ArrayList<Expression>.Empty;
        }

        private static void CheckChildRegistrations(IResolverPipeline parentDependencyGraph, Dictionary<Type, Registration> registrations, object syncRoot)
        {
            lock (syncRoot)
            {
                foreach (Registration registration in registrations.Values)
                {
                    Type type = registration.DependencyType;
                    if (parentDependencyGraph.Dependencies.TryGetValue(type, out _))
                    {
                        throw new RegistrationAlreadyExistsException($"Dependency {type} was already registered in the parent graph!");
                    }
                    else if (type.IsGenericType)
                    {
                        if (parentDependencyGraph.Dependencies.TryGetValue(type.GetGenericTypeDefinition(), out _))
                        {
                            throw new RegistrationAlreadyExistsException($"Dependency {type} was already registered as a open generic in the parent graph!");
                        }
                    }
                }
            }
        }
    }
}
