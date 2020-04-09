using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Singularity.Resolving.Generators
{
    /// <summary>
    /// Creates bindings so that open generic registrations can be properly resolved.
    /// </summary>
    public sealed class OpenGenericBindingGenerator : IGenericServiceGenerator
    {
        private static Type[] _excludedGenericTypes = new Type[] { typeof(HashSet<>), typeof(ISet<>), typeof(List<>), typeof(IList<>), typeof(ICollection<>), typeof(Lazy<>), typeof(Func<>), typeof(Expression<>), typeof(IEnumerable<>), typeof(IReadOnlyCollection<>), typeof(IReadOnlyList<>) };

        public bool CanResolve(Type type)
        {
            return type.IsGenericType && !type.ContainsGenericParameters && !_excludedGenericTypes.Contains(type.GetGenericTypeDefinition());
        }

        /// <inheritdoc />
        public IEnumerable<ServiceBinding> Wrap(IInstanceFactoryResolver resolver, Type type)
        {
            Type genericTypeDefinition = type.GetGenericTypeDefinition();
            ServiceBinding? openGenericBinding = resolver.TryGetBinding(genericTypeDefinition);
            if (openGenericBinding != null)
            {
                Type openGenericType = openGenericBinding.ConcreteType;
                Type closedGenericType = openGenericType.MakeGenericType(type.GenericTypeArguments);
                Expression? newExpression = openGenericBinding.ConstructorResolver.TryResolveConstructorExpression(closedGenericType);

                yield return new ServiceBinding(type, openGenericBinding.BindingMetadata, newExpression, closedGenericType,
                    openGenericBinding.ConstructorResolver, openGenericBinding.Lifetime, openGenericBinding.Finalizer,
                    openGenericBinding.NeedsDispose);
            }
        }
    }
}