using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
        private readonly IGenericGenerator[] _genericWrapperGenerators = new IGenericGenerator[]
        {
            new FactoryServiceBindingGenerator(),
            new LazyServiceBindingGenerator(),
            new ExpressionServiceBindingGenerator(),
            new EnumerableWrapperGenerator(),
            new ArrayWrapperGenerator(),
            new ListWrapperGenerator(),
            new SetWrapperGenerator(),
            new ScopedFuncGenerator(),
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

        private static readonly MethodInfo GenericResolveMethod = typeof(InstanceFactoryResolver).GetRuntimeMethods().Single(x => x.Name == nameof(RunGenerators) && x.ContainsGenericParameters);
        public IEnumerable<ServiceBinding> RunGenerators<TUnwrapped, TWrapped>(IGenericGenerator genericGenerator)
        {
            switch (genericGenerator)
            {
                case IGenericWrapperGenerator genericWrapperGenerator:
                    {
                        var dependentType = genericWrapperGenerator.DependsOn(typeof(TUnwrapped)) ?? typeof(TUnwrapped);
                        var unwrappedBindings = FindOrGenerateApplicableBindings(dependentType);
                        foreach (var unwrappedBinding in unwrappedBindings)
                        {
                            var factory = TryResolveDependency(dependentType, unwrappedBinding);
                            if (factory != null)
                            {
                                var expression = genericWrapperGenerator.Wrap<TUnwrapped, TWrapped>(factory.Context.Expression, dependentType);
                                if (!typeof(TWrapped).IsAssignableFrom(expression.Type)) throw new InvalidOperationException($"Expression {expression} is not assignable to {typeof(TWrapped)}");
                                var binding = new ServiceBinding(typeof(TWrapped), BindingMetadata.GeneratedInstance, expression, typeof(TWrapped), ConstructorResolvers.Default, Lifetimes.Transient);
                                yield return binding;
                            }
                        }
                    }
                    break;
                case IGenericServiceGenerator genericServiceGenerator:
                    {
                        var expression = genericServiceGenerator.Wrap(this, typeof(TWrapped));
                        var binding = new ServiceBinding(typeof(TWrapped), BindingMetadata.GeneratedInstance, expression, typeof(TWrapped), ConstructorResolvers.Default, Lifetimes.Transient);
                        yield return binding;
                    }
                    break;
                default:
                    throw new NotSupportedException($"The generator of type {genericGenerator.GetType()} is not supported");
            }
        }

        public IEnumerable<ServiceBinding> FindOrGenerateApplicableBindings(Type type)
        {
            lock (SyncRoot)
            {
                var genericGenerator = _genericWrapperGenerators.SingleOrDefault(x => x.CanResolve(type));
                if (genericGenerator != null)
                {
                    var unWrappedType = type.IsArray ? type.GetElementType() : type.GetGenericArguments().Last();
                    foreach (var item in (IEnumerable<ServiceBinding>)GenericResolveMethod.MakeGenericMethod(unWrappedType, type).Invoke(this, new[] { genericGenerator }))
                    {
                        yield return item;
                    }
                }
                else
                {
                    var bindings = RegistrationStore.Registrations.Where(x => x.Key == type).SelectMany(x => x.Value).Distinct();
                    if (_parentPipeline != null)
                    {
                        bindings = bindings.Concat(_parentPipeline.FindApplicableBindings(type));
                    }
                    if (bindings.Any())
                    {
                        foreach (var item in bindings)
                        {
                            yield return item;
                        }
                    }
                    else
                    {
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
                                    yield return binding;
                                }
                                yield break;
                            }
                        }
                    }
                }
            }
        }

        public IEnumerable<ServiceBinding> FindApplicableBindings(Type type)
        {
            var bindings = RegistrationStore.Registrations.Where(x => x.Key == type).SelectMany(x => x.Value).Distinct();
            if (_parentPipeline != null)
            {
                bindings = bindings.Concat(_parentPipeline.FindApplicableBindings(type).Where(x => x.BindingMetadata.Generated == false));
            }
            return bindings;
        }

        public ServiceBinding? TryGetBinding(Type type) => FindOrGenerateApplicableBindings(type).FirstOrDefault();

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

        public InstanceFactory? TryResolveDependency(Type type, ServiceBinding dependency) => TryResolveDependency(type, dependency, new CircularDependencyDetector());

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

        private static void CheckChildRegistrations(Dictionary<Type, SinglyLinkedListNode<ServiceBinding>> parentRegistrations, Dictionary<Type, SinglyLinkedListNode<ServiceBinding>> childRegistrations, object syncRoot)
        {
            lock (syncRoot)
            {
                foreach (KeyValuePair<Type, SinglyLinkedListNode<ServiceBinding>> registration in childRegistrations)
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
