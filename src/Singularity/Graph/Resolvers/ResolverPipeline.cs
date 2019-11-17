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
        public SingularitySettings Settings { get; }
        private RegistrationStore RegistrationStore { get; }
        private object SyncRoot { get; }
        private readonly IServiceBindingGenerator[] _resolvers;
        private readonly ResolverPipeline? _parentPipeline;
        private readonly Scoped _containerScope;
        private readonly ExpressionGenerator _expressionGenerator = new ExpressionGenerator();

        public ResolverPipeline(RegistrationStore registrationStore, Scoped containerScope, SingularitySettings settings, ResolverPipeline? parentPipeline)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _resolvers = Settings.ServiceBindingGenerators.ToArray();
            _parentPipeline = parentPipeline;
            SyncRoot = parentPipeline?.SyncRoot ?? new object();
            _containerScope = containerScope ?? throw new ArgumentNullException(nameof(containerScope));
            RegistrationStore = registrationStore ?? throw new ArgumentNullException(nameof(registrationStore));

            if (parentPipeline != null)
            {
                CheckChildRegistrations(parentPipeline.RegistrationStore.Registrations, registrationStore.Registrations, SyncRoot);
            }
        }

        public InstanceFactory Resolve(Type type)
        {
            if (Settings.ResolveErrorsExclusions.Match(type))
            {
                return TryResolve(type)!;
            }
            ServiceBinding serviceBinding = GetDependency(type).Default;
            return ResolveDependency(type, serviceBinding);
        }

        public InstanceFactory? TryResolve(Type type)
        {
            ServiceBinding? serviceBinding = TryGetDependency(type)?.Default;
            return serviceBinding == null ? null : TryResolveDependency(type, serviceBinding);
        }

        public IEnumerable<InstanceFactory> TryResolveAll(Type type)
        {
            Registration? registration = TryGetDependency(type);
            if (registration == null) yield break;
            foreach (ServiceBinding registrationBinding in registration.Value.Bindings.Array)
            {
                var factory = TryResolveDependency(type, registrationBinding);
                if (factory != null) yield return factory;
            }
        }

        private Registration? TryGetDependency(Type type)
        {
            lock (SyncRoot)
            {
                if (RegistrationStore.Registrations.TryGetValue(type, out Registration parent)) return parent;

                foreach (IServiceBindingGenerator dependencyResolver in _resolvers)
                {
                    if (Settings.ResolverExclusions.TryGetValue(dependencyResolver.GetType(), out var exclusions))
                    {
                        if (exclusions.Any(x => x.Match(type))) continue;
                    }
                    ServiceBinding[] serviceBindings = dependencyResolver.Resolve(this, type).ToArray();
                    if (serviceBindings.Length > 0)
                    {
                        foreach (ServiceBinding binding in serviceBindings)
                        {
                            RegistrationStore.AddBinding(binding);
                        }
                        return TryGetDependency(type);
                    }
                }

                return _parentPipeline?.TryGetDependency(type);
            }
        }

        private Registration GetDependency(Type type)
        {
            Registration? dependency = TryGetDependency(type);
            if (dependency == null) throw new DependencyNotFoundException(type);
            return dependency.Value;
        }

        private InstanceFactory ResolveDependency(Type type, ServiceBinding dependency)
        {
            InstanceFactory? factory = TryResolveDependency(type, dependency);
            if (factory != null)
            {
                return factory;
            }

            throw dependency.ResolveError ?? throw new NotImplementedException("Factory was null but the resolve exception was null as well");
        }

        private InstanceFactory? TryResolveDependency(Type type, ServiceBinding dependency) => TryResolveDependency(type, dependency, new CircularDependencyDetector());

        private InstanceFactory? TryResolveDependency(Type type, ServiceBinding dependency, CircularDependencyDetector circularDependencyDetector)
        {
            try
            {
                Settings.Logger.Log($"{nameof(TryResolveDependency)} for {type}", circularDependencyDetector.Count);
                circularDependencyDetector.Enter(type);
                GenerateBaseExpression(dependency, circularDependencyDetector);
                InstanceFactory factory = GenerateInstanceFactory(type, dependency, circularDependencyDetector);
                return factory;
            }
            catch (Exception e)
            {
                if (e is DependencyResolveException dependencyResolveException)
                {
                    e = dependencyResolveException.InnerException;
                }
                dependency.ResolveError = new DependencyResolveException(type, dependency, e);
                return null;
            }
            finally
            {
                circularDependencyDetector.Leave(type);
            }
        }

        private void GenerateBaseExpression(ServiceBinding serviceBinding, CircularDependencyDetector circularDependencyDetector)
        {
            if (serviceBinding.BaseExpression == null)
            {
                lock (serviceBinding)
                {
                    if (serviceBinding.BaseExpression == null)
                    {
                        if (serviceBinding.Expression == null)
                        {
                            var constructor = serviceBinding.ConstructorResolver.DynamicSelectConstructor(serviceBinding.ConcreteType, this);
                            serviceBinding.Expression = serviceBinding.ConcreteType.ResolveConstructorExpression(constructor);
                        }
                        ParameterExpression[] parameters = serviceBinding.Expression.GetParameterExpressions().Where(x => x.Type != ExpressionGenerator.ScopeParameter.Type).ToArray();
                        var factories = new InstanceFactory[parameters.Length];
                        for (var i = 0; i < parameters.Length; i++)
                        {
                            ParameterExpression parameter = parameters[i];
                            ServiceBinding child = GetDependency(parameter.Type).Default;
                            InstanceFactory? factory = TryResolveDependency(parameter.Type, child, circularDependencyDetector);
                            factories[i] = factory ?? throw (child.ResolveError ?? throw new NotImplementedException());
                        }

                        serviceBinding.BaseExpression = _expressionGenerator.GenerateBaseExpression(serviceBinding, factories, _containerScope, Settings);
                    }
                }
            }
        }


        private InstanceFactory GenerateInstanceFactory(Type type, ServiceBinding serviceBinding, CircularDependencyDetector circularDependencyDetector)
        {
            lock (serviceBinding)
            {
                if (!serviceBinding.TryGetInstanceFactory(type, out InstanceFactory factory))
                {
                    if (serviceBinding.Expression is AbstractBindingExpression)
                    {
                        factory = new InstanceFactory(type, (ExpressionContext)serviceBinding.BaseExpression!, scoped => throw new AbstractTypeResolveException($"Cannot create a instance for type {type} since its registered as a abstract binding and not meant to be used directly."));
                        serviceBinding.Factories.Add(factory);
                        return factory;
                    }

                    Expression[] decorators = FindDecorators(type);
                    ParameterExpression[] parameters = decorators.GetParameterExpressions().Where(x => x.Type != ExpressionGenerator.ScopeParameter.Type && x.Type != type).ToArray();
                    var childFactories = new InstanceFactory[parameters.Length];
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        ParameterExpression parameter = parameters[i];
                        ServiceBinding child = GetDependency(parameter.Type).Default;
                        var childFactory = TryResolveDependency(parameter.Type, child, circularDependencyDetector);
                        childFactories[i] = childFactory ?? throw (child.ResolveError ?? throw new NotImplementedException());
                    }

                    ReadOnlyExpressionContext context = _expressionGenerator.ApplyDecorators(type, serviceBinding, childFactories, decorators, _containerScope);
                    factory = new InstanceFactory(type, context);
                    serviceBinding.Factories.Add(factory);
                }
                return factory;
            }
        }

        private Expression[] FindDecorators(Type type)
        {
            return RegistrationStore.Decorators.TryGetValue(type, out ArrayList<Expression> decorators) ? decorators.Array : ArrayList<Expression>.Empty;
        }

        private static void CheckChildRegistrations(Dictionary<Type, Registration> parentRegistrations, Dictionary<Type, Registration> childRegistrations, object syncRoot)
        {
            lock (syncRoot)
            {
                foreach (KeyValuePair<Type, Registration> registration in childRegistrations)
                {
                    Type type = registration.Key;
                    if (parentRegistrations.TryGetValue(type, out _))
                    {
                        throw new RegistrationAlreadyExistsException($"Dependency {type} was already registered in the parent graph!");
                    }
                    else if (type.IsGenericType && parentRegistrations.TryGetValue(type.GetGenericTypeDefinition(), out _))
                    {
                        throw new RegistrationAlreadyExistsException($"Dependency {type} was already registered as a open generic in the parent graph!");
                    }
                }
            }
        }
    }
}
