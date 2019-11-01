﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Singularity.Expressions;

namespace Singularity.Graph.Resolvers
{
    public sealed class FactoryDependencyResolver : IDependencyResolver
    {
        public IEnumerable<ServiceBinding> Resolve(IResolverPipeline graph, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Func<>))
            {
                Type dependencyType = type.GenericTypeArguments[0];

                foreach (InstanceFactory factory in graph.TryResolveAll(dependencyType))
                {
                    LambdaExpression baseExpression = Expression.Lambda(factory.Context.Expression);

                    yield return new ServiceBinding(type, BindingMetadata.GeneratedInstance, baseExpression, graph.Settings.ConstructorResolver)
                    {
                        BaseExpression = new ExpressionContext(baseExpression)
                    };
                }
            }
        }
    }
}
