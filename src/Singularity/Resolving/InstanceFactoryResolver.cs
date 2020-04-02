using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Singularity.Collections;
using Singularity.Exceptions;
using Singularity.Expressions;
using Singularity.Resolving.Generators;

namespace Singularity.Resolving
{
    internal sealed class InstanceFactoryResolver : IInstanceFactoryResolver
    {
        public SingularitySettings Settings { get; }
        private RegistrationStore RegistrationStore { get; }
        private object SyncRoot { get; }
        private readonly IServiceBindingGenerator[] _resolvers;
        private readonly IGenericWrapperGenerator[] _genericWrapperGenerators = new IGenericWrapperGenerator[]
        {
            new FactoryServiceBindingGenerator(),
            new LazyServiceBindingGenerator(),
            new ExpressionServiceBindingGenerator(),
        };
        private readonly InstanceFactoryResolver? _parentPipeline;
        private readonly Scoped _containerScope;
        private readonly ExpressionGenerator _expressionGenerator = new ExpressionGenerator();

        public InstanceFactoryResolver(RegistrationStore registrationStore, Scoped containerScope, SingularitySettings settings, InstanceFactoryResolver? parentPipeline)
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
                // TODO Bit of a hack since the signature of this method means it cannot return null.. However this is needed for integration with microsoft dependency injection.
                // The reason this is needed is because the of way IServiceProvider is used in ASP .NET it has to return null in some cases instead of throwing a exception.
                // Ideally IServiceProvider should have different methods for this so this hack is not needed.
                return TryResolve(type) ?? new InstanceFactory(type, new ReadOnlyExpressionContext(new ExpressionContext(Expression.Constant(null))));
            }

            ServiceBinding serviceBinding = GetBinding(type);
            return ResolveDependency(type, serviceBinding);
        }

        public InstanceFactory? TryResolve(Type type)
        {
            ServiceBinding? serviceBinding = TryGetBinding(type);
            return serviceBinding == null ? null : TryResolveDependency(type, serviceBinding);
        }

        public IEnumerable<Type> GetResolvableTypes()
        {
            var valid = RegistrationStore.Registrations.SelectMany(x => x.Value.Bindings).Select(x => x.ConcreteType).Distinct().ToArray();

            var foo = valid.ToList();
            foreach (var genericWrapper in _genericWrapperGenerators)
            {
                foreach (var type in valid)
                {
                    foo.Add(genericWrapper.Target(type));
                }                
            }
            return foo;
        }

        //// TODO move to collectionservicebindinggenerator..
        //public IEnumerable<InstanceFactory> TryResolveAll(Type type)
        //{
        //    var genericWrapper = _genericWrapperGenerators.FirstOrDefault(x => x.CanResolve(type));

        //    if (genericWrapper != null)
        //    {
        //        var unwrappedType = genericWrapper.Target(type);
        //        TryResolveAll(unwrappedType);

        //    }
        //    else
        //    {
        //        var valid = RegistrationStore.Registrations.SelectMany(x => x.Value.Bindings).Where(x => type.IsAssignableFrom(x.ConcreteType)).Distinct().ToArray();

        //        foreach (ServiceBinding registrationBinding in valid)
        //        {
        //            var factory = TryResolveDependency(type, registrationBinding);
        //            if (factory != null) yield return factory;
        //        }
        //    }
        //}

        public ServiceBinding? TryGetBinding(Type type)
        {
            lock (SyncRoot)
            {
                if (RegistrationStore.Registrations.TryGetValue(type, out Registration parent)) return parent.Default;
                var parentDependency = _parentPipeline?.TryGetRegistration(type);
                if (parentDependency != null)
                {
                    if (!parentDependency.Value.Default.BindingMetadata.Generated) return parentDependency.Value.Default;
                }

                var genericWrapper = _genericWrapperGenerators.FirstOrDefault(x => x.CanResolve(type));

                if (genericWrapper != null)
                {
                    var innerType = type.GetGenericArguments()[0];
                    var dependentType = genericWrapper.DependsOn(innerType) ?? innerType;
                    var serviceBinding = TryGetBinding(dependentType);
                    if(serviceBinding != null)
                    {
                        var factory = TryResolveDependency(innerType, serviceBinding);
                        if (factory != null)
                        {
                            var expression = genericWrapper.Wrap(factory.Context.Expression, dependentType, type);

                            return new ServiceBinding(type, BindingMetadata.GeneratedInstance, expression, type, ConstructorResolvers.Default, Lifetimes.Transient)
                            {
                                BaseExpression = new ExpressionContext(expression)
                            };
                        }
                    }
                }

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
                        return TryGetBinding(type);
                    }
                }

                return null;
            }
        }

        private Registration? TryGetRegistration(Type type)
        {
            lock (SyncRoot)
            {
                if (RegistrationStore.Registrations.TryGetValue(type, out Registration parent)) return parent;

                return _parentPipeline?.TryGetRegistration(type);
            }
        }

        private ServiceBinding GetBinding(Type type)
        {
            return TryGetBinding(type) ?? throw new DependencyNotFoundException(type);
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
                if (type.IsGenericType && type.ContainsGenericParameters)
                {
                    throw new OpenGenericTypeResolveException($"Cannot create a instance for type {type} since its registered as a abstract binding and not meant to be used directly.");
                }
                GenerateBaseExpression(dependency, circularDependencyDetector);
                InstanceFactory factory = GenerateInstanceFactory(type, dependency, circularDependencyDetector);
                return factory;
            }
            catch (Exception e)
            {
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
                        InstanceFactory[] factories = ResolveParameters(parameters, circularDependencyDetector);

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
                    Expression[] decorators = FindDecorators(type);
                    ParameterExpression[] parameters = decorators.GetParameterExpressions().Where(x => x.Type != ExpressionGenerator.ScopeParameter.Type && x.Type != type).ToArray();
                    InstanceFactory[] factories = ResolveParameters(parameters, circularDependencyDetector);
                    ReadOnlyExpressionContext context = _expressionGenerator.ApplyDecorators(type, serviceBinding, factories, decorators, _containerScope);
                    factory = new InstanceFactory(type, context);
                    serviceBinding.Factories.Add(factory);
                }
                return factory;
            }
        }

        private InstanceFactory[] ResolveParameters(ParameterExpression[] parameters, CircularDependencyDetector circularDependencyDetector)
        {
            var factories = new InstanceFactory[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                ParameterExpression parameter = parameters[i];
                ServiceBinding child = GetBinding(parameter.Type);
                InstanceFactory? factory = TryResolveDependency(parameter.Type, child, circularDependencyDetector);
                factories[i] = factory ?? throw (child.ResolveError ?? throw new NotImplementedException());
            }

            return factories;
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
