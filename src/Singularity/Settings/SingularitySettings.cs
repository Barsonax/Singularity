using System;
using System.Collections.Generic;
using System.ComponentModel;
using Singularity.Expressions;
using Singularity.Graph.Resolvers;
using Singularity.Logging;
using Singularity.Settings;

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
        public static SingularitySettings Microsoft => new SingularitySettings()
            .AutoDispose(Lifetimes.Transient, Lifetimes.PerContainer, Lifetimes.PerScope, Lifetimes.PerGraph);

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

        private SingularitySettings() { }

        public SingularitySettings IgnoreResolveError(IMatch match)
        {
            ResolveErrorsExclusions.Add(match);
            return this;
        }

        public SingularitySettings ExcludeAutoRegistration(Type type, IMatch match)
        {
            if (!ResolverExclusions.TryGetValue(type, out var exclusions))
            {
                exclusions = new List<IMatch>();
                ResolverExclusions.Add(type, exclusions);
            }
            exclusions.Add(match);
            return this;
        }

        public SingularitySettings With(ISingularityLogger logger)
        {
            Logger = logger;
            return this;
        }

        public SingularitySettings With(IDependencyResolver[] resolvers)
        {
            Resolvers = resolvers;
            return this;
        }

        public SingularitySettings With(IConstructorResolver constructorResolver)
        {
            ConstructorResolver = constructorResolver;
            return this;
        }

        public SingularitySettings AutoDispose(params ILifetime[] autoDisposeLifetimes)
        {
            foreach (ILifetime autoDisposeLifetime in autoDisposeLifetimes)
            {
                AutoDispose(autoDisposeLifetime);
            }

            return this;
        }

        public SingularitySettings AutoDispose(ILifetime lifetime)
        {
            AutoDisposeLifetimes.Add(lifetime.GetType());
            return this;
        }
    }
}
