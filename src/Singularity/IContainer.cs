using System;
using System.Collections.Generic;
using Singularity.Exceptions;

namespace Singularity
{
    /// <summary>
    /// Interface for a singularity container.
    /// </summary>
    public interface IContainer : IDisposable, IServiceProvider
    {
        /// <summary>
        /// Resolves a instance for the given dependency type
        /// </summary>
        /// <typeparam name="T">The type of the dependency</typeparam>
        /// <exception cref="DependencyNotFoundException">If the dependency is not configured</exception>
        /// <returns></returns>
        T GetInstance<T>() where T : class;

        /// <summary>
        /// Resolves a instance for the given dependency type
        /// </summary>
        /// <param name="type">The type of the dependency</param>
        /// <exception cref="DependencyNotFoundException">If the dependency is not configured</exception>
        /// <returns></returns>
        object GetInstance(Type type);

        /// <summary>
        /// Injects dependencies by calling all methods that were registered using <see cref="ContainerBuilder.LateInject{T}"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <exception cref="DependencyNotFoundException">If the late injection target had parameters that couldn't be resolved</exception>
        void LateInject(object instance);

        /// <summary>
        /// Resolves a instance for the given dependency type
        /// </summary>
        /// <typeparam name="T">The type of the dependency</typeparam>
        /// <returns></returns>
        T? GetInstanceOrDefault<T>() where T : class;

        /// <summary>
        /// Resolves a instance for the given dependency type
        /// </summary>
        /// <param name="type">The type of the dependency</param>
        /// <returns></returns>
        object? GetInstanceOrDefault(Type type);

        /// <summary>
        /// Injects dependencies for all elements of the provided <paramref name="instances"/> by calling all late injection targets that were registered using <see cref="ContainerBuilder.LateInject{T}"/>
        /// </summary>
        /// <param name="instances"></param>
        /// <exception cref="DependencyNotFoundException">If the late injection target had parameters that couldn't be resolved</exception>
        void LateInjectAll<T>(IEnumerable<T> instances);
    }
}