using System;
using System.Collections.Generic;
using Singularity.Exceptions;

namespace Singularity.Graph.Resolvers
{
    /// <summary>
    /// Interface resolving services.
    /// </summary>
    public interface IResolverPipeline
    {
        /// <summary>
        /// The settings
        /// </summary>
        SingularitySettings Settings { get; }

        /// <summary>
        /// Resolves the factory for a type.
        /// Throws if it fails except if a exclusion has been added to <see cref="SingularitySettings.ResolveErrorsExclusions"/>, in this case a null will be returned.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="DependencyResolveException"></exception>
        /// <exception cref="DependencyNotFoundException"></exception>
        InstanceFactory Resolve(Type type);

        /// <summary>
        /// Tries to resolve the factory for a type.
        /// Returns null if it fails.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        InstanceFactory? TryResolve(Type type);

        /// <summary>
        /// Tries to resolve all the factories for a type, returning a <see cref="IEnumerable{T}"/>
        /// If one of the factories fail to resolve it will be skipped.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IEnumerable<InstanceFactory> TryResolveAll(Type type);
    }
}