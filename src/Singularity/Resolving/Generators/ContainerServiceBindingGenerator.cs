﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Collections;
using Singularity.Expressions;

namespace Singularity.Resolving.Generators
{
    /// <summary>
    /// Creates bindings for resolving the container or scope itself.
    /// </summary>
    public class ContainerServiceBindingGenerator : IServiceBindingGenerator
    {
        private static readonly MethodInfo _getContainer = typeof(ContainerServiceBindingGenerator).GetMethod(nameof(GetContainer), BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly MethodInfo _getScope = typeof(ContainerServiceBindingGenerator).GetMethod(nameof(GetScope), BindingFlags.NonPublic | BindingFlags.Static);

        /// <inheritdoc />
        public IEnumerable<ServiceBinding> Resolve(IInstanceFactoryResolver resolver, Type type)
        {
            if (type == typeof(Container))
            {
                Expression expression = Expression.Call(null, _getContainer, ExpressionGenerator.ScopeParameter);
                yield return new ServiceBinding(typeof(Container), BindingMetadata.GeneratedInstance, expression, typeof(Container), ConstructorResolvers.Default, Lifetimes.PerContainer, null, ServiceAutoDispose.Never);
            }

            if (type == typeof(Scoped) || type == typeof(IServiceProvider))
            {
                Expression expression = Expression.Call(null, _getScope, ExpressionGenerator.ScopeParameter);
                yield return new ServiceBinding(new SinglyLinkedListNode<Type>(typeof(IServiceProvider)).Add(typeof(Scoped)), BindingMetadata.GeneratedInstance, expression, typeof(Scoped), ConstructorResolvers.Default, Lifetimes.PerContainer, null, ServiceAutoDispose.Never);
            }
        }

        private static Container GetContainer(Scoped scope)
        {
            return scope.Container;
        }

        private static Scoped GetScope(Scoped scope)
        {
            return scope;
        }
    }
}
