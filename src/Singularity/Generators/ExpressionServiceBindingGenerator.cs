﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Bindings;
using Singularity.Expressions;
using Singularity.Resolvers;

namespace Singularity.Generators
{
    /// <summary>
    /// Creates bindings so that the expression itself of a binding can be resolved
    /// </summary>
    public sealed class ExpressionServiceBindingGenerator : IServiceBindingGenerator
    {
        private static readonly MethodInfo GenericResolveMethod = typeof(ExpressionServiceBindingGenerator).GetRuntimeMethods().Single(x => x.Name == nameof(TryGenerate) && x.ContainsGenericParameters);

        /// <inheritdoc />
        public IEnumerable<ServiceBinding> TryGenerate(IContainerContext context, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Expression<>) && type.GenericTypeArguments.Length == 1)
            {
                Type funcType = type.GenericTypeArguments[0];
                if (funcType.GetGenericTypeDefinition() == typeof(Func<>) && funcType.GenericTypeArguments.Length == 1)
                {
                    Type dependencyType = funcType.GenericTypeArguments[0];
                    MethodInfo resolveMethod = GenericResolveMethod.MakeGenericMethod(dependencyType);

                    var bindings = (IEnumerable<ServiceBinding>)resolveMethod.Invoke(this, new object[] { context, type });
                    foreach (var binding in bindings)
                    {
                        yield return binding;
                    }
                }
            }
        }

        private IEnumerable<ServiceBinding> TryGenerate<TElement>(IContainerContext graph, Type type)
        {
            foreach (InstanceFactory instanceFactory in graph.TryResolveAll(typeof(TElement)))
            {
                var newBinding = new ServiceBinding(type, BindingMetadata.GeneratedInstance, instanceFactory.Context.Expression, instanceFactory.Context.Expression.Type, ConstructorResolvers.Default, Lifetimes.Transient);

                var expression = Expression.Lambda<Func<TElement>>(ExpressionCompiler.OptimizeExpression(instanceFactory.Context));
                Func<Scoped, Expression<Func<TElement>>> del = scoped => expression; // we need to put this in a variable of this type or cast it else the static return type of the delegate will turn into a object..
                var factory = new InstanceFactory(type, new ExpressionContext(expression), del);
                newBinding.Factories.Add(factory);
                yield return newBinding;
            }
        }
    }
}