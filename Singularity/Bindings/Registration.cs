using System;
using System.Collections.Generic;
using Singularity.Collections;
using Singularity.Exceptions;

namespace Singularity.Bindings
{
    internal class Registration
    {
        public Type[] DependencyTypes { get; }
        public SinglyLinkedListNode<WeaklyTypedBinding>? Bindings { get; set; }

        public Registration(Type[] dependencyTypes)
        {
            DependencyTypes = dependencyTypes ?? throw new ArgumentNullException(nameof(dependencyTypes));
        }

        internal void Verify(List<WeaklyTypedDecoratorBinding>? decorators)
        {
            SinglyLinkedListNode<WeaklyTypedBinding>? currentBinding = Bindings;
            while (currentBinding != null)
            {
                if (currentBinding.Value.Expression == null && (decorators == null || decorators!.Count == 0))
                {
                    throw new BindingConfigException($"The binding at {currentBinding.Value.BindingMetadata.StringRepresentation()} does not have a expression");
                }

                if (decorators != null)
                {
                    foreach (WeaklyTypedDecoratorBinding weaklyTypedDecoratorBinding in decorators)
                    {
                        if (weaklyTypedDecoratorBinding.Expression == null)
                            throw new BindingConfigException($"The decorator for {DependencyTypes} does not have a expression");
                    }
                }

                currentBinding = currentBinding.Next;
            }
        }
    }
}