using Singularity.Collections;
using Singularity.Expressions;
using System;
using System.Linq;

namespace Singularity.Resolving
{
    public class ServiceContext
    {
        public ServiceContext(ServiceBinding binding)
        {
            this.Binding = binding;
        }

        public ServiceBinding Binding { get; }

        /// <summary>
        /// The base expression of to create a instance of this service. Dependencies are resolved and the lifetime has been applied, however decorators have not yet been applied.
        /// Do not use this directly but use <see cref="Factories"/> instead.
        /// </summary>
        public ReadOnlyExpressionContext? BaseExpression { get; internal set; }

        /// <summary>
        /// Will store the error if a error occurs when resolving the service.
        /// </summary>
        public Exception? ResolveError { get; internal set; }

        /// <summary>
        /// A list of factories for different service types.
        /// These factories are generated using the <see cref="BaseExpression"/> as input.
        /// Decorators are applied at this point and its safe to use this to resolve dependencies of other services with <see cref="TryGetInstanceFactory"/>
        /// </summary>
        public ArrayList<InstanceFactory> Factories { get; } = new ArrayList<InstanceFactory>();

        /// <summary>
        /// Gets the instance factory if it exists for the given type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public bool TryGetInstanceFactory(Type type, out InstanceFactory factory)
        {
            factory = Factories.Array.FirstOrDefault(x => x.ServiceType == type);
            return factory != null;
        }
    }
}
