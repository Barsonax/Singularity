using System;
using System.Collections.Generic;
using Singularity.Exceptions;

namespace Singularity.Bindings
{
    internal class Registration
    {
        public Type DependencyType { get; }
        public List<WeaklyTypedBinding> Bindings { get; } = new List<WeaklyTypedBinding>();

        public Registration(Type dependencyType)
        {
            DependencyType = dependencyType ?? throw new ArgumentNullException(nameof(dependencyType));
        }

        internal void Verify(List<WeaklyTypedDecoratorBinding>? decorators)
        {
            foreach (WeaklyTypedBinding weaklyTypedBinding in Bindings)
            {
                if (weaklyTypedBinding.Expression == null && (decorators == null || decorators!.Count == 0))
                {
                    throw new BindingConfigException($"The binding at {weaklyTypedBinding.BindingMetadata.StringRepresentation()} does not have a expression");
                }

                if (decorators != null)
                {
                    foreach (WeaklyTypedDecoratorBinding weaklyTypedDecoratorBinding in decorators)
                    {
                        if (weaklyTypedDecoratorBinding.Expression == null)
                            throw new BindingConfigException($"The decorator for {DependencyType} does not have a expression");
                    }
                }
            }
        }
    }
}