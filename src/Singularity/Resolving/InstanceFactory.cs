using System;

using Singularity.Expressions;

namespace Singularity.Resolving
{
    /// <summary>
    /// Factory for creating instances of a service for a service type.
    /// </summary>
    public sealed class InstanceFactory
    {
        /// <summary>
        /// The service type.
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// Expression that is used to create the instance of the service
        /// </summary>
        public ReadOnlyExpressionContext Context { get; }

        private Func<Scoped, object?>? _factory;

        /// <summary>
        /// <see cref="Context"/> compiled into a generic delegate.
        /// </summary>
        public Func<Scoped, object?> Factory => _factory ??= ExpressionCompiler.Compile(Context);

        /// <summary>
        /// Creates a new <see cref="InstanceFactory"/>
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="context"></param>
        /// <param name="factory"></param>
        public InstanceFactory(Type serviceType, ReadOnlyExpressionContext context, Func<Scoped, object>? factory = null)
        {
            ServiceType = serviceType;
            Context = context;
            _factory = factory;
        }
    }
}