using System;
using System.Collections.Generic;
using System.ComponentModel;
using Singularity.Expressions;
using Singularity.Graph.Resolvers;
using Singularity.Logging;

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
        public static SingularitySettings Microsoft => new SingularitySettings(c =>
                                                         c.AutoDispose(Lifetimes.Transient, Lifetimes.PerContainer, Lifetimes.PerScope, Lifetimes.PerGraph));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IDependencyResolver[] Resolvers { get; private set; } = {
            new ContainerDependencyResolver(),
            new EnumerableDependencyResolver(),
            new ExpressionDependencyResolver(),
            new LazyDependencyResolver(),
            new FactoryDependencyResolver(),
            new ConcreteDependencyResolver(),
            new OpenGenericResolver()
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

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ISingularityLogger Logger { get; private set; } = Loggers.Default;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<IMatch> ResolveErrorsExclusions { get; } = new List<IMatch>();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Dictionary<Type, List<IMatch>> ResolverExclusions { get; } = new Dictionary<Type, List<IMatch>>();

        private SingularitySettings(Action<SingularitySettings> configurator = null) { }

        public void IgnoreResolveError(IMatch match)
        {
            ResolveErrorsExclusions.Add(match);
        }

        public void ExcludeAutoRegistration(Type type, IMatch match)
        {
            if (!ResolverExclusions.TryGetValue(type, out var exclusions))
            {
                exclusions = new List<IMatch>();
                ResolverExclusions.Add(type, exclusions);
            }
            exclusions.Add(match);
        }

        public void With(ISingularityLogger logger) => Logger = logger;

        public void With(IDependencyResolver[] resolvers) => Resolvers = resolvers;

        public void With(IConstructorResolver constructorResolver) => ConstructorResolver = constructorResolver;

        public void AutoDispose(params ILifetime[] autoDisposeLifetimes)
        {
            foreach (ILifetime autoDisposeLifetime in autoDisposeLifetimes)
            {
                AutoDispose(autoDisposeLifetime);
            }
        }
        public void AutoDispose(ILifetime lifetime) => AutoDisposeLifetimes.Add(lifetime.GetType());
    }
}
