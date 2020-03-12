using System;
using System.Collections.Generic;
using System.ComponentModel;

using Singularity.Collections;
using Singularity.Expressions;
using Singularity.Logging;
using Singularity.Resolving.Generators;

namespace Singularity
{
    /// <summary>
    /// Defines settings for the singularity container.
    /// </summary>
    public sealed class SingularitySettings
    {
        /// <summary>
        /// The default singularity settings.
        /// </summary>
        public static SingularitySettings Default => new SingularitySettings();

        /// <summary>
        /// Settings for microsoft dependency injection.
        /// </summary>
        public static SingularitySettings Microsoft => new SingularitySettings(s =>
        {
            s.AutoDispose(Lifetimes.Transient, Lifetimes.PerContainer, Lifetimes.PerScope, Lifetimes.PerGraph);
            s.IgnoreResolveError(new PatternTypeMatcher("Microsoft.*"));
        });

        /// <summary>
        /// The <see cref="IServiceBindingGenerator"/>s that are used to dynamically generate bindings.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ConfigurationList<IServiceBindingGenerator> ServiceBindingGenerators { get; } = new ConfigurationList<IServiceBindingGenerator> {
            new ContainerServiceBindingGenerator(),
            new CollectionServiceBindingGenerator(),
            new ExpressionServiceBindingGenerator(),
            new LazyServiceBindingGenerator(),
            new FactoryServiceBindingGenerator(),
            new ConcreteServiceBindingGenerator(),
            new OpenGenericBindingGenerator()
        };

        /// <summary>
        /// Specifies what lifetimes should be auto disposed if the instance is a <see cref="IDisposable"/> and the service is registered with <see cref="ServiceAutoDispose.Default"/>.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public HashSet<Type> AutoDisposeLifetimes { get; } = new HashSet<Type>();

        /// <summary>
        /// The constructor selector that will be used by default.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IConstructorResolver ConstructorResolver { get; private set; } = ConstructorResolvers.Default;

        /// <summary>
        /// The logger that is used.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ISingularityLogger Logger { get; private set; } = Loggers.Default;

        /// <summary>
        /// Prevents Singularity to throw errors in some cases
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<ITypeMatcher> ResolveErrorsExclusions { get; } = new List<ITypeMatcher>();

        /// <summary>
        /// Excludes some types from a <see cref="IServiceBindingGenerator"/>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Dictionary<Type, List<ITypeMatcher>> ResolverExclusions { get; } = new Dictionary<Type, List<ITypeMatcher>>();

        private SingularitySettings(Action<SingularitySettings>? configurator = null) { configurator?.Invoke(this); }

        /// <summary>
        /// Configures the <see cref="IServiceBindingGenerator"/>s that will be used.
        /// </summary>
        /// <param name="configurator"></param>
        public void ConfigureServiceBindingGenerators(Action<ConfigurationList<IServiceBindingGenerator>> configurator)
        {
            configurator.Invoke(ServiceBindingGenerators);
        }

        /// <summary>
        /// Ignores any error during resolving a type and returns null if the type matches with the passed <paramref name="match"/>.
        /// </summary>
        /// <param name="match"></param>
        public void IgnoreResolveError(ITypeMatcher match)
        {
            ResolveErrorsExclusions.Add(match);
        }

        /// <summary>
        /// Prevents a <see cref="IServiceBindingGenerator"/> from being executed if the to be resolved type matches with <paramref name="match"/>
        /// </summary>
        /// <typeparam name="TResolverType"></typeparam>
        /// <param name="match"></param>
        public void ExcludeAutoRegistration<TResolverType>(ITypeMatcher match)
            where TResolverType : IServiceBindingGenerator
        {
            ExcludeAutoRegistration(typeof(TResolverType), match);
        }

        /// <summary>
        /// Prevents the <paramref name="serviceBindingGeneratorType"/>> from being executed if the to be resolved type matches with <paramref name="match"/>
        /// </summary>
        /// <param name="serviceBindingGeneratorType"></param>
        /// <param name="match"></param>
        public void ExcludeAutoRegistration(Type serviceBindingGeneratorType, ITypeMatcher match)
        {
            if (!ResolverExclusions.TryGetValue(serviceBindingGeneratorType, out List<ITypeMatcher> exclusions))
            {
                exclusions = new List<ITypeMatcher>();
                ResolverExclusions.Add(serviceBindingGeneratorType, exclusions);
            }
            exclusions.Add(match);
        }

        /// <summary>
        /// Changes the logger.
        /// </summary>
        /// <param name="logger"></param>
        public void With(ISingularityLogger logger) => Logger = logger;

        /// <summary>
        /// Changes the default <see cref="IConstructorResolver"/>
        /// </summary>
        /// <param name="constructorResolver"></param>
        public void With(IConstructorResolver constructorResolver) => ConstructorResolver = constructorResolver;

        /// <summary>
        /// Automatically disposes instances that use the passed <paramref name="lifetimes"/>
        /// </summary>
        /// <param name="lifetimes"></param>
        public void AutoDispose(params ILifetime[] lifetimes)
        {
            foreach (ILifetime autoDisposeLifetime in lifetimes)
            {
                AutoDispose(autoDisposeLifetime);
            }
        }

        /// <summary>
        /// Automatically disposes instances that use the passed <paramref name="lifetime"/>
        /// </summary>
        /// <param name="lifetime"></param>
        public void AutoDispose(ILifetime lifetime) => AutoDisposeLifetimes.Add(lifetime.GetType());
    }
}
